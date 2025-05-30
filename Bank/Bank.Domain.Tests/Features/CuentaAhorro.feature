# Scenario: Cliente cancela su cuenta con saldo cero y es correcto
#	Given la nueva cuenta numero 12345
#	When cancelo la cuenta
#	Then la cuenta deberia estar cancelada

# Scenario: Cliente cancela su cuenta con saldo y es incorrecto
#	Given la nueva cuenta numero 12345
#    And con saldo 10
#	When cancelo la cuenta
#	Then deberia ser error
#	And deberia mostrarse el error: No se puede cancelar una cuenta con saldo