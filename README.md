# SESION DE LABORATORIO N° 09: PRUEBAS DE ACEPTACION DE USUARIO CON BDD

### Nombre:

## OBJETIVOS
  * Comprender el funcionamiento de las pruebas unitarias dentro de una aplicación utilizando el Framework de pruebas NUnit.

## REQUERIMIENTOS
  * Conocimientos: 
    - Conocimientos básicos de Bash (powershell).
    - Conocimientos básicos de Contenedores (Docker).
  * Hardware:
    - Virtualization activada en el BIOS..
    - CPU SLAT-capable feature.
    - Al menos 4GB de RAM.
  * Software:
    - Windows 10 64bit: Pro, Enterprise o Education (1607 Anniversary Update, Build 14393 o Superior)
    - Docker Desktop 
    - Powershell versión 7.x
    - Net 6 o superior
    - Visual Studio Code

## CONSIDERACIONES INICIALES
  * Clonar el repositorio mediante git para tener los recursos necesarios

## DESARROLLO
1. Iniciar la aplicación Powershell o Windows Terminal en modo administrador 
2. Ejecutar el siguiente comando para crear una nueva solución
```
dotnet new sln -o Bank
```
3. Acceder a la solución creada y ejecutar el siguiente comando para crear una nueva libreria de clases y adicionarla a la solución actual.
```
cd Bank
dotnet new classlib -o Bank.Domain
dotnet sln add ./Bank.Domain/Bank.Domain.csproj
```
4. Ejecutar el siguiente comando para crear un nuevo proyecto de pruebas y adicionarla a la solución actual
```
dotnet new nunit -o Bank.Domain.Tests
dotnet sln add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj package SpecFlow.NUnit
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj package SpecFlow.Plus.LivingDocPlugin
dotnet add ./Bank.Domain.Tests/Bank.Domain.Tests.csproj reference ./Bank.Domain/Bank.Domain.csproj
```
5. Iniciar Visual Studio Code (VS Code) abriendo el folder de la solución como proyecto. En el proyecto Bank.Domain, si existe un archivo Class1.cs proceder a eliminarlo. Asimismo en el proyecto Bank.Domain.Tests si existiese un archivo UnitTest1.cs, también proceder a eliminarlo.

6. En VS Code, en el proyecto Bank.Domain proceder a crear  e introducir el siguiente código:
> Cliente.cs
```C#
namespace Bank.Domain
{
    public class Cliente
    {
        public int IdCliente { get; private set; }
        public string NombreCliente { get; private set; }
        public static Cliente Registrar(string _nombre)
        {
            return new Cliente(){
                NombreCliente = _nombre
            };
        }   
    }
}
```  
> CuentaAhorro.cs
```C#
namespace Bank.Domain
{
    public class CuentaAhorro
    {
        public const string ERROR_MONTO_MENOR_IGUAL_A_CERO = "El monto no puede ser menor o igual a 0";
        public int IdCuenta { get; private set; }
        public string NumeroCuenta { get; private set; }
        public virtual Cliente Propietario { get; private set; }
        public int IdPropietario { get; private set; }
        public decimal Tasa { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime FechaApertura { get; private set; }
        public bool Estado { get; private set; }
        
        public static CuentaAhorro Aperturar(string _numeroCuenta, Cliente _propietario, decimal _tasa)
        {
            return new CuentaAhorro()
            {
                NumeroCuenta = _numeroCuenta,
                Propietario = _propietario,
                IdPropietario = _propietario.IdCliente,
                Tasa = _tasa,
                Saldo = 0
            };
        }     
        public void Depositar(decimal monto)
        {
            if (monto <= 0)
                throw new Exception (ERROR_MONTO_MENOR_IGUAL_A_CERO);
            Saldo += monto;
        }
        public void Retirar(decimal monto)
        {
            if (monto <= 0)
                throw new Exception (ERROR_MONTO_MENOR_IGUAL_A_CERO);
            Saldo -= monto;
        }
    }
}
```
7. Luego en el proyecto Bank.Domain.Tests añadir un nuevo folder con nombre `Features`.
8. Dentro de la carpeta Features crear el archivo BanckAccountTests.cs e introducir el siguiente código:
```C#
using Bank.Domain;
using NUnit.Framework;
using TechTalk.SpecFlow;
namespace Bank.Domain.Tests.Features
{
    [Binding]
    public sealed class CuentaAhorroPruebas
    {
        private readonly ScenarioContext _scenarioContext;
        private CuentaAhorro _cuenta { get; set; }
        private string _error { get; set; }
        private bool _es_error { get; set; } = false;
        
        public CuentaAhorroPruebas(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("la nueva cuenta numero (.*)")]
        public void DadoUnaNuevaCuenta(string numeroCuenta)
        {
            try
            {
                var cliente = Cliente.Registrar("Juan Perez");
                _cuenta = CuentaAhorro.Aperturar(numeroCuenta, cliente, 1);
            }
            catch (System.Exception ex)
            {
                _es_error = true; 
                _error = ex.Message;
            }            
        }

        // [Given("con saldo (.*)")]
        // public void YConSaldo(decimal monto)
        // {
        //     CuandoYoDeposito(monto);
        // }

        [Given("con saldo (.*)")]
        [When("deposito (.*)")]
        public void CuandoYoDeposito(decimal monto)
        {
            try
            {
                _cuenta.Depositar(monto);
            }
            catch (System.Exception ex)
            {
                _es_error = true; 
                _error = ex.Message;
            }
        }

        [When("retiro (.*)")]
        public void CuandoYoRetiro(decimal monto)
        {
            try
            {
                _cuenta.Retirar(monto);
                //_resultado = _cuenta.Saldo;
            }
            catch (System.Exception ex)
            {
                _es_error = true; 
                _error = ex.Message;
            }
        }

        [Then("el saldo nuevo deberia ser (.*)")]
        public void EntoncesElResultadoDeberiaSer(decimal resultado)
        {
            Assert.AreEqual(_cuenta.Saldo, resultado);
        }        

        [Then("deberia ser error")]
        public void EntoncesDeberiaMostrarseError()
        {
            Assert.IsTrue(_es_error);
        }

        [Then("deberia mostrarse el error: (.*)")]
        public void EntoncesDeberiaMostrarseError(string error)
        {
            Assert.AreEqual(_error, error);
        }
    }
}
```
9. Luego en el proyecto Bank.Domain.Tests añadir un nuevo folder con nombre `Steps`.
10. Dentro de la carpeta Steps crear el archivo CuentaAhorro.feature e introducir el siguiente código:
```Gherkin
Feature: Como cliente quiero realizar depositos y retiros para modificar mi saldo de cuenta

Scenario: Cliente deposita en su cuenta un monto y es correcto
	Given la nueva cuenta numero 12345
	When deposito 10
	Then el saldo nuevo deberia ser 10

Scenario: Cliente retira en su cuenta un monto y es correcto
	Given la nueva cuenta numero 12345
    And con saldo 10
	When retiro 10
	Then el saldo nuevo deberia ser 0

Scenario: Cliente retira en su cuenta un monto negativo y es incorrecto
	Given la nueva cuenta numero 12345
    And con saldo 10
	When retiro -10
	Then deberia ser error
```  
11. Abrir un terminal en VS Code (CTRL + Ñ) o vuelva al terminal anteriormente abierto, y ejecutar los comandos:
```Bash
dotnet build
dotnet test --collect:"XPlat Code Coverage"
```
12. El resultado debe ser similar al siguiente. 
```Bash
Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     3, Duration: 12 ms
```
13. Finalmente proceder a verificar la cobertura, dentro del proyecto Primes.Tests se dede haber generado una carpeta o directorio TestResults, en el cual posiblemente exista otra subpcarpeta o subdirectorio conteniendo un archivo con nombre `coverage.cobertura.xml`, si existe ese archivo proceder a ejecutar los siguientes comandos desde la linea de comandos abierta anteriomente, de los contrario revisar el paso 8:
```
dotnet tool install -g dotnet-reportgenerator-globaltool
ReportGenerator "-reports:./*/*/*/coverage.cobertura.xml" "-targetdir:Cobertura" -reporttypes:HTML
```
14. El comando anterior primero proceda instalar una herramienta llamada ReportGenerator (https://reportgenerator.io/) la cual mediante la segunda parte del comando permitira generar un reporte en formato HTML con la cobertura obtenida de la ejecución de las pruebas. Este reporte debe localizarse dentro de una carpeta llamada Cobertura y puede acceder a el abriendo con un navegador de internet el archivo index.htm.

15. Finalmente proceder a verificar las pruebas en base a comportamiento, para esto ejecutar el siguiente comando en el terminal anteriormente abierto:
```
dotnet tool install --global SpecFlow.Plus.LivingDoc.CLI
livingdoc test-assembly .\Bank.Domain.Tests\bin\Debug\net7.0\Bank.Domain.Tests.dll -t .\Bank.Domain.Tests\bin\Debug\net7.0\TestExecution.json -o BankTests.html
```
16. El comando anterior primero proceda instalar una herramienta llamada Specflow +LivingDoc  (https://specflow.org/tools/living-doc/) la cual mediante la segunda parte del comando permitira generar un reporte en formato HTML con las pruebas en base a comportamiento creadas. Este reporte debe localizarse en el mismo directorio donde se encuentra actualmente y puede acceder a el abriendo con un navegador de internet el archivo index.htm.

---
## Actividades Encargadas
1. Adicionar en la clase CuentaAhorro un metodo que se llame Cancelar, adicionar dos nuevos escenarios de pruebas para el metodo Cancelar.
2. Completar la documentación del Clases, atributos y métodos para luego generar una automatización (publish_docs.yml) que genere la documentación utilizando DocFx y la publique en una Github Page
3. Generar una automatización (publish_bdd_report.yml) que: * Compile el proyecto y ejecute las pruebas, * Genere el reporte BDD, * Publique el reporte en Github Page
4. Generar una automatización (release.yml) que: * Genere el nuget con su codigo de matricula como version del componente, * Publique el nuget en Github Packages, * Genere el release correspondiente
