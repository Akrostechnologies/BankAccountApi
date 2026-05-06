# Data Transfer Objects (DTOs)

# Data Transfer Objects (DTOs)

<details>
<summary>Relevant source files</summary>

The following files were used as context for generating this wiki page:

- [src/Api/Dtos/BankAccountDto.cs](src/Api/Dtos/BankAccountDto.cs)

</details>



The `BankAccountApi` utilizes Data Transfer Objects (DTOs) to define the schema of data sent over the network. This layer of abstraction decouples the internal **Domain Model** from the external **API Contract**, ensuring that changes to the database schema or business logic entities do not inadvertently break client integrations.

## The BankAccountDto Class

The primary DTO currently implemented is the `BankAccountDto`, located in the `Api.Dtos` namespace [src/Api/Dtos/BankAccountDto.cs:4-6](). It serves as a simplified, serializable representation of the `BankAccount` entity.

### Properties
The DTO exposes a specific subset of data required by the API consumers:

| Property | Type | Description | Source Entity Field |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | The unique identifier for the account. | `bankAccount.Id` |
| `AccountNumber` | `string` | The public-facing account string. | `bankAccount.AccountNumber` |
| `AccountBalance` | `decimal` | The current monetary balance. | `bankAccount.AccountBalance` |

**Sources:** [src/Api/Dtos/BankAccountDto.cs:8-10]()

## Projection and Mapping

To facilitate efficient data transformation, the `BankAccountDto` includes a static projection expression. This allows the application to map Domain entities to DTOs directly within LINQ queries, which is particularly useful when working with Entity Framework to ensure only required columns are fetched from the data store.

### FromBankAccount Expression
The `FromBankAccount` property is defined as a `static Expression<Func<BankAccount, BankAccountDto>>` [src/Api/Dtos/BankAccountDto.cs:12-18](). 

By using an `Expression` instead of a compiled `Func`, the mapping logic can be parsed by IQueryable providers (like EF Core). This ensures that the transformation happens at the database level (SQL `SELECT` statement) rather than fetching the entire entity into memory before converting it.

**Sources:** [src/Api/Dtos/BankAccountDto.cs:1-18]()

## Data Flow and Decoupling

The DTO acts as a buffer between the `Domain.Models.BankAccount` entity and the `BankAccountController`. This separation of concerns provides several architectural benefits:
1.  **Version Stability**: The internal `BankAccount` model can add fields (e.g., internal audit timestamps or private status flags) without changing the JSON structure returned to the user.
2.  **Security**: It prevents "Overposting" attacks by explicitly defining which fields can be sent or received, ensuring sensitive internal state is never exposed.
3.  **Performance**: It reduces payload size by excluding unnecessary domain logic or navigation properties during serialization.

### Logic to DTO Mapping Diagram
The following diagram illustrates how the system transitions from the Domain Space to the API Contract Space using the `FromBankAccount` projection.

Title: Domain to DTO Projection Flow
```mermaid
graph LR
    subgraph "Domain Model Space"
        [BankAccount_Entity] -- "Contains Domain Logic" --> Entity["BankAccount"]
    end

    subgraph "Transformation Layer"
        Entity -- "Passed to" --> [FromBankAccount_Expression]
        [FromBankAccount_Expression] -- "LINQ Projection" --> Mapping["new BankAccountDto { ... }"]
    end

    subgraph "API Contract Space"
        Mapping --> [BankAccountDto_Instance]
        [BankAccountDto_Instance] -- "Serialized to JSON" --> HTTP_Response["HTTP 200 OK"]
    end

    style Entity stroke-width:2px
    style [BankAccountDto_Instance] stroke-width:2px
```
**Sources:** [src/Api/Dtos/BankAccountDto.cs:6-18](), [src/Api/Controllers/BankAccountController.cs:1-35]()

### Entity vs DTO Structural Mapping
This diagram bridges the system names to the specific code entities involved in the data transfer process.

Title: Structural Association between Entity and DTO
```mermaid
classDiagram
    class "BankAccount" {
        <<Domain.Models>>
        +int Id
        +string AccountNumber
        +decimal AccountBalance
    }

    class "BankAccountDto" {
        <<Api.Dtos>>
        +int Id
        +string AccountNumber
        +decimal AccountBalance
        +FromBankAccount Expression
    }

    "BankAccount" ..> "BankAccountDto" : "Projected via FromBankAccount"
```
**Sources:** [src/Api/Dtos/BankAccountDto.cs:6-18]()

---