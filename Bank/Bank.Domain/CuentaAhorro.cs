namespace Bank.Domain
{
    /// <summary>
    /// Representa una cuenta de ahorro bancaria con operaciones básicas.
    /// </summary>
    public class CuentaAhorro
    {
        /// <summary>
        /// Mensaje de error cuando se intenta realizar una operación con un monto menor o igual a cero.
        /// </summary>
        public const string ERROR_MONTO_MENOR_IGUAL_A_CERO = "El monto no puede ser menor o igual a 0";
        
        /// <summary>
        /// Mensaje de error cuando se intenta cancelar una cuenta con saldo.
        /// </summary>
        public const string ERROR_CANCELAR_CUENTA_CON_SALDO = "No se puede cancelar una cuenta con saldo";
        
        /// <summary>
        /// Identificador único de la cuenta.
        /// </summary>
        public int IdCuenta { get; private set; }
        
        /// <summary>
        /// Número de cuenta asignado.
        /// </summary>
        public string NumeroCuenta { get; private set; }
        
        /// <summary>
        /// Cliente propietario de la cuenta.
        /// </summary>
        public virtual Cliente Propietario { get; private set; }
        
        /// <summary>
        /// Identificador del propietario de la cuenta.
        /// </summary>
        public int IdPropietario { get; private set; }
        
        /// <summary>
        /// Tasa de interés aplicada a la cuenta.
        /// </summary>
        public decimal Tasa { get; private set; }
        
        /// <summary>
        /// Saldo actual de la cuenta.
        /// </summary>
        public decimal Saldo { get; private set; }
        
        /// <summary>
        /// Fecha en que se aperturó la cuenta.
        /// </summary>
        public DateTime FechaApertura { get; private set; }
        
        /// <summary>
        /// Estado de la cuenta (true = activa, false = cancelada).
        /// </summary>
        public bool Estado { get; private set; }
        
        /// <summary>
        /// Crea una nueva instancia de cuenta de ahorro.
        /// </summary>
        /// <param name="_numeroCuenta">Número de cuenta a asignar.</param>
        /// <param name="_propietario">Cliente propietario de la cuenta.</param>
        /// <param name="_tasa">Tasa de interés a aplicar.</param>
        /// <returns>Una nueva instancia de CuentaAhorro.</returns>
        public static CuentaAhorro Aperturar(string _numeroCuenta, Cliente _propietario, decimal _tasa)
        {
            return new CuentaAhorro()
            {
                NumeroCuenta = _numeroCuenta,
                Propietario = _propietario,
                IdPropietario = _propietario.IdCliente,
                Tasa = _tasa,
                Saldo = 0,
                FechaApertura = DateTime.Now,
                Estado = true
            };
        }     
        
        /// <summary>
        /// Realiza un depósito en la cuenta.
        /// </summary>
        /// <param name="monto">Monto a depositar.</param>
        /// <exception cref="Exception">Se lanza cuando el monto es menor o igual a cero.</exception>
        public void Depositar(decimal monto)
        {
            if (monto <= 0)
                throw new Exception(ERROR_MONTO_MENOR_IGUAL_A_CERO);
            Saldo += monto;
        }
        
        /// <summary>
        /// Realiza un retiro de la cuenta.
        /// </summary>
        /// <param name="monto">Monto a retirar.</param>
        /// <exception cref="Exception">Se lanza cuando el monto es menor o igual a cero.</exception>
        public void Retirar(decimal monto)
        {
            if (monto <= 0)
                throw new Exception(ERROR_MONTO_MENOR_IGUAL_A_CERO);
            Saldo -= monto;
        }
        
        /// <summary>
        /// Cancela la cuenta de ahorro.
        /// </summary>
        /// <exception cref="Exception">Se lanza cuando se intenta cancelar una cuenta con saldo.</exception>
        public void Cancelar()
        {
            if (Saldo > 0)
                throw new Exception(ERROR_CANCELAR_CUENTA_CON_SALDO);
            Estado = false;
        }
    }
}