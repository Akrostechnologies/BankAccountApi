# API Reference

# API Reference

<details>
<summary>Relevant source files</summary>

The following files were used as context for generating this wiki page:

- [src/Api/Controllers/BankAccountApiController.cs](src/Api/Controllers/BankAccountApiController.cs)
- [src/Api/Dtos/BankAccountDto.cs](src/Api/Dtos/BankAccountDto.cs)

</details>



The `BankAccountApi` exposes a RESTful HTTP surface designed to manage banking operations. The API layer acts as the entry point for external clients, utilizing controllers to handle incoming requests and Data Transfer Objects (DTOs) to define the contract for data exchange.

## HTTP Interface Overview

The API is built using ASP.NET Core `ControllerBase` patterns [src/Api/Controllers/BankAccountApiController.cs:12-12](). It follows standard REST conventions where the resource path is determined by the controller name [src/Api/Controllers/BankAccountApiController.cs:11-11]().

### Request Flow and Data Transformation

The following diagram illustrates how a request moves from the network through the API layer and how data is projected from Domain Entities into DTOs.

**API Request to Domain Mapping**
```mermaid
graph TD
    subgraph "External Space"
        "Client" -- "HTTP GET /BankAccount/TestGet" --> "API_Endpoint"
    end

    subgraph "Code Entity Space (Api Project)"
        "API_Endpoint" --> "BankAccountController"["BankAccountController"]
        "BankAccountController" --> "TestGet_Method"["TestGet()"]
        "TestGet_Method" -- "Uses" --> "BankAccountDto"["BankAccountDto"]
    end

    subgraph "Code Entity Space (Domain Project)"
        "BankAccountDto" -- "Maps From" --> "BankAccount_Entity"["BankAccount Entity"]
    end

    "BankAccountDto" -- "Expression" --> "FromBankAccount"["FromBankAccount Expression"]
```
Sources: [src/Api/Controllers/BankAccountApiController.cs:10-28](), [src/Api/Dtos/BankAccountDto.cs:6-18]()

## Routing and Controllers

The primary entry point for banking operations is the `BankAccountController`. Currently, the API surface is in a scaffolded state, providing a base for future business logic implementation.

*   **Base Route**: `[controller]` (resolves to `/BankAccount`) [src/Api/Controllers/BankAccountApiController.cs:11-11]().
*   **Logging**: The controller utilizes `ILogger<BankAccountController>` for diagnostic tracing [src/Api/Controllers/BankAccountApiController.cs:14-14]().
*   **Endpoints**: Currently contains a `TestGet` stub used for verifying API availability [src/Api/Controllers/BankAccountApiController.cs:21-27]().

For details on endpoint definitions and planned logic, see [BankAccountController](#3.1).

Sources: [src/Api/Controllers/BankAccountApiController.cs:1-28]()

## Data Transfer Objects (DTOs)

To prevent leaking internal domain models to the public API surface, the project uses DTOs. This decoupling allows the `BankAccount` entity to evolve independently of the JSON contract returned to clients.

*   **Contract Definition**: The `BankAccountDto` defines the public shape of an account, including `Id`, `AccountNumber`, and `AccountBalance` [src/Api/Dtos/BankAccountDto.cs:8-10]().
*   **Projection Logic**: The class contains a static `FromBankAccount` LINQ expression [src/Api/Dtos/BankAccountDto.cs:12-12](). This is used to efficiently project `BankAccount` domain entities directly into DTOs during database queries [src/Api/Dtos/BankAccountDto.cs:13-18]().

For details on the DTO structure and mapping, see [Data Transfer Objects (DTOs)](#3.2).

Sources: [src/Api/Dtos/BankAccountDto.cs:1-19]()

## API Contract Architecture

The relationship between the API controllers and the data contracts is managed within the `Api` project.

**Entity to DTO Relationship**
```mermaid
classDiagram
    class "BankAccount" {
        +int Id
        +string AccountNumber
        +decimal AccountBalance
    }
    class "BankAccountDto" {
        +int Id
        +string AccountNumber
        +decimal AccountBalance
        +FromBankAccount(BankAccount) BankAccountDto
    }
    class "BankAccountController" {
        +TestGet() IActionResult
    }

    "BankAccountController" ..> "BankAccountDto" : "Returns"
    "BankAccountDto" ..> "BankAccount" : "Projects from"
```
Sources: [src/Api/Controllers/BankAccountApiController.cs:12-28](), [src/Api/Dtos/BankAccountDto.cs:6-18]()

---