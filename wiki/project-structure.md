# Project Structure

# Project Structure

<details>
<summary>Relevant source files</summary>

The following files were used as context for generating this wiki page:

- [BankAccountApi.sln](BankAccountApi.sln)
- [src/Api/Api.csproj](src/Api/Api.csproj)
- [src/Application/Application.csproj](src/Application/Application.csproj)
- [src/Domain/Domain.csproj](src/Domain/Domain.csproj)
- [src/Infrastructure/Infrastructure.csproj](src/Infrastructure/Infrastructure.csproj)

</details>



This page describes the organizational layout of the `BankAccountApi` solution. The project follows Clean Architecture principles, separating concerns into distinct layers to ensure maintainability and testability. The solution is partitioned into source code (`src/`) and test suites (`tests/`).

## Solution Layout

The solution is organized into logical folders using Visual Studio solution folders to group related projects.

### Folder Conventions
- **src/**: Contains the functional source code of the application, divided into four layers.
- **tests/**: Contains unit testing projects corresponding to the source layers.
- **Solution Items**: Contains root-level documentation such as the `README.md`.

### Dependency Flow Diagram
The following diagram illustrates how projects depend on one another. Note that the `Domain` layer remains the core and has no outward dependencies, while the `Api` project acts as the entry point and composition root.

**Layered Dependency Map**
```mermaid
graph TD
    subgraph "src/"
        [Api.csproj] --> [Application.csproj]
        [Api.csproj] --> [Infrastructure.csproj]
        [Api.csproj] --> [Domain.csproj]
        
        [Application.csproj] --> [Infrastructure.csproj]
        [Application.csproj] --> [Domain.csproj]
        
        [Infrastructure.csproj] --> [Domain.csproj]
        
        [Domain.csproj]
    end

    subgraph "tests/"
        [ApiUnitTests.csproj] --> [Api.csproj]
        [DomainUnitTests.csproj] --> [Domain.csproj]
        [InfrastructureUnitTests.csproj] --> [Infrastructure.csproj]
    end
```
**Sources:** [BankAccountApi.sln:6-28](), [src/Api/Api.csproj:14-18](), [src/Application/Application.csproj:9-12](), [src/Infrastructure/Infrastructure.csproj:13-15]()

---

## Source Projects (`src/`)

### 1. Domain Layer (`src/Domain`)
The core of the system. It contains the enterprise logic and entities.
*   **Role**: Defines the fundamental business models (e.g., `BankAccount`) and service interfaces (e.g., `IBankAccountService`).
*   **Dependencies**: None. This project is a pure .NET library without external framework dependencies.

### 2. Infrastructure Layer (`src/Infrastructure`)
Handles technical concerns and external data persistence.
*   **Role**: Implements the data access logic using Entity Framework Core. It contains the `BankDbContext` and handles the in-memory database configuration.
*   **Key Dependencies**: `Microsoft.EntityFrameworkCore.InMemory`.
*   **Internal Dependencies**: References `Domain`.

### 3. Application Layer (`src/Application`)
Coordinates the business flow.
*   **Role**: Implements the `IBankAccountService` defined in the Domain. It acts as a bridge between the Domain logic and the Infrastructure data access.
*   **Internal Dependencies**: References `Domain` and `Infrastructure`.

### 4. API Layer (`src/Api`)
The entry point of the application.
*   **Role**: An ASP.NET Core Web API project. It contains controllers (e.g., `BankAccountController`), DTOs, and the `Program.cs` file which configures the Dependency Injection (DI) container.
*   **Key Dependencies**: `Swashbuckle.AspNetCore` for Swagger documentation.
*   **Internal Dependencies**: References all other layers (`Application`, `Domain`, `Infrastructure`) to perform service registration.

**Sources:** [src/Domain/Domain.csproj:1-9](), [src/Infrastructure/Infrastructure.csproj:1-17](), [src/Application/Application.csproj:1-14](), [src/Api/Api.csproj:1-20]()

---

## Test Projects (`tests/`)

The testing strategy utilizes **MSTest** and **Moq** to validate individual layer behavior in isolation.

| Project | Target Project | Purpose |
| :--- | :--- | :--- |
| `ApiUnitTests` | `Api` | Tests HTTP endpoints and controller logic using `NullLoggerFactory` and mocks. |
| `DomainUnitTests` | `Domain` | Validates business rules and entity state transitions. |
| `InfrastructureUnitTests` | `Infrastructure` | Validates data persistence and `DbContext` behavior. |

**Sources:** [BankAccountApi.sln:21-26](), [BankAccountApi.sln:71-73]()

---

## Code Entity Mapping

This diagram maps the architectural layers to specific code entities and files within the project structure to help navigate the codebase.

**Code Entity Relationship Map**
```mermaid
graph LR
    subgraph "External/UI"
        [BankAccountController]
    end

    subgraph "Application Logic"
        [BankAccountService]
        [IBankAccountService]
    end

    subgraph "Data & State"
        [BankDbContext]
        [BankAccount]
    end

    [BankAccountController] -- "invokes" --> [IBankAccountService]
    [BankAccountService] -- "implements" --> [IBankAccountService]
    [BankAccountService] -- "queries" --> [BankDbContext]
    [BankDbContext] -- "manages" --> [BankAccount]
    [BankAccountController] -- "returns" --> [BankAccountDto]
```

| Layer | Primary Entity | File Path Reference |
| :--- | :--- | :--- |
| **Api** | `BankAccountController` | `src/Api/Controllers/BankAccountController.cs` |
| **Application** | `BankAccountService` | `src/Application/BankAccountService.cs` |
| **Domain** | `IBankAccountService` | `src/Domain/Interfaces/IBankAccountService.cs` |
| **Infrastructure** | `BankDbContext` | `src/Infrastructure/BankDbContext.cs` |

**Sources:** [src/Api/Api.csproj:14-18](), [src/Application/Application.csproj:9-12](), [src/Infrastructure/Infrastructure.csproj:13-15](), [BankAccountApi.sln:11-28]()

---