# Proyecto de Cuenta Bancaria
## Resumen
Este ejercicio está diseñado para probar tu capacidad de crear una API RESTful para la gestión de cuentas bancarias. Deberás crear puntos de acceso que permitan a los usuarios obtener información de la cuenta, añadir depósitos, realizar retiros y transferir dinero entre cuentas. También necesitarás implementar un proceso para calcular el interés diario sobre los saldos de las cuentas.

## Criterios de Aceptación
Crear una API RESTful que permita a los usuarios:
Obtener información de la cuenta bancaria, incluyendo saldo actual, lista de transacciones e interés ganado
Añadir depósitos a una cuenta
Crear retiros de una cuenta
Transferir dinero entre cuentas


### Crédito Extra - Proceso de Diseño
Para implementar la función de interés diario, necesitarás crear una función que se ejecute diariamente para calcular el interés ganado por cada titular de cuenta. También necesitarás crear una tabla de base de datos o un modelo de datos que rastree el saldo de la cuenta, la tasa de interés y la fecha del último cálculo de interés. La función debe calcular el interés ganado desde la última fecha de cálculo y actualizar el saldo de la cuenta en consecuencia.

## Instrucciones
1. Clona este repositorio en tu máquina local.
2. Implementa la API RESTful según los criterios de aceptación.
3. Crea un modelo de datos o tabla de base de datos para rastrear los saldos de las cuentas, las tasas de interés y las fechas del último cálculo de interés.
4. Implementa el proceso de cálculo de interés diario (ver sección de crédito extra arriba).
5. Escribe pruebas para tu implementación

Notas:

- Puedes modificar los modelos/arquitectura del proyecto a medida que resuelvas el problema para ayudar a escribir un código limpio. Esta solución es solo un punto de partida.
- Este ejercicio no está destinado a ser completado totalmente en el límite de 2 horas, vamos a usar este tiempo para avanzar lo más que podamos. Concéntrate en la calidad del código, las pruebas y la comunicación mientras resuelves el problema.


## Criterios de Evaluación
- La implementación debe cumplir con todos los criterios de aceptación mencionados anteriormente.
- El código debe estar bien organizado y ser fácil de leer.
- Usa convenciones de nomenclatura apropiadas para variables, funciones y puntos de acceso.
- El código debe estar bien documentado con comentarios claros y/o archivos README.
- Aunque esto es un ejercicio, la implementación debe tratar de ser segura y proteger contra vulnerabilidades comunes como inyección SQL y ataques de scripting entre sitios (XSS).
