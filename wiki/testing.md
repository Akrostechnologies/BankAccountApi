# Testing

# Testing

<details>
<summary>Relevant source files</summary>

The following files were used as context for generating this wiki page:

- [tests/ApiUnitTests/ApiUnitTests.csproj](tests/ApiUnitTests/ApiUnitTests.csproj)
- [tests/DomainUnitTests/DomainUnitTests.csproj](tests/DomainUnitTests/DomainUnitTests.csproj)
- [tests/InfrastructureUnitTests/InfrastructureUnitTests.csproj](tests/InfrastructureUnitTests/InfrastructureUnitTests.csproj)

</details>



The `BankAccountApi` solution includes a dedicated testing suite designed to validate each layer of the Clean Architecture independently. The testing strategy utilizes **MSTest** as the primary test framework, **Moq** for dependency isolation, and **Coverlet** for code coverage collection [tests/ApiUnitTests/ApiUnitTests.csproj:12-16]().

The test suite is divided into three distinct projects, mirroring the source code structure to ensure separation of concerns between API contracts, business logic, and data infrastructure.

### Test Architecture Overview

The following diagram illustrates the relationship between the test projects and the source projects they target.

**Test Project Dependencies**
```mermaid
graph TD
    subgraph "Test_Projects"
        ["ApiUnitTests"]
        ["DomainUnitTests"]
        ["InfrastructureUnitTests"]
    end

    subgraph "Source_Projects"
        ["Api_Project"]
        ["Domain_Project"]
        ["Infrastructure_Project"]
    end

    ["ApiUnitTests"] --> ["Api_Project"]
    ["ApiUnitTests"] --> ["Domain_Project"]
    ["DomainUnitTests"] -.-> ["Domain_Project"]
    ["InfrastructureUnitTests"] -.-> ["Infrastructure_Project"]

    style ["ApiUnitTests"] stroke-dasharray: 0
    style ["DomainUnitTests"] stroke-dasharray: 5
    style ["InfrastructureUnitTests"] stroke-dasharray: 5
```
**Sources:** [tests/ApiUnitTests/ApiUnitTests.csproj:19-22](), [tests/DomainUnitTests/DomainUnitTests.csproj:1-18](), [tests/InfrastructureUnitTests/InfrastructureUnitTests.csproj:1-18]()

---

### Test Project Suites

The testing infrastructure is categorized into three main projects within the `tests/` directory:

| Project | Target Layer | Key Dependencies | Status |
| :--- | :--- | :--- | :--- |
| `ApiUnitTests` | `Api` | `MSTest`, `Moq`, `Microsoft.NET.Test.Sdk` | Active |
| `DomainUnitTests` | `Domain` | `MSTest`, `coverlet.collector` | Scaffolded |
| `InfrastructureUnitTests` | `Infrastructure` | `MSTest`, `coverlet.collector` | Scaffolded |

#### API Unit Tests
The `ApiUnitTests` project is the most developed suite, focusing on the `BankAccountController`. It utilizes `Moq` to simulate the behavior of domain services and `NullLoggerFactory` to satisfy logging requirements without side effects. For details, see [API Unit Tests](#4.1).

**Sources:** [tests/ApiUnitTests/ApiUnitTests.csproj:11-17]()

#### Domain & Infrastructure Unit Tests
The `DomainUnitTests` and `InfrastructureUnitTests` projects are currently in a scaffolded state. They contain the necessary SDK references and configuration for `net6.0` but primarily host placeholder test stubs. These projects are intended to eventually validate domain entities, business rules, and database context configurations. For details, see [Domain & Infrastructure Unit Tests](#4.2).

**Sources:** [tests/DomainUnitTests/DomainUnitTests.csproj:4-16](), [tests/InfrastructureUnitTests/InfrastructureUnitTests.csproj:4-16]()

---

### Testing Frameworks and Tooling

The solution standardizes on the following technical stack for quality assurance:

*   **MSTest (TestAdapter & TestFramework):** The engine used to discover and execute tests [tests/ApiUnitTests/ApiUnitTests.csproj:14-15]().
*   **Moq:** Used specifically in the API layer to mock `IBankAccountService` and other abstractions, allowing for isolated testing of HTTP responses and routing logic [tests/ApiUnitTests/ApiUnitTests.csproj:13]().
*   **Coverlet:** Integrated via `coverlet.collector` to provide cross-platform code coverage analysis [tests/ApiUnitTests/ApiUnitTests.csproj:16]().

**Framework Mapping**
```mermaid
graph LR
    subgraph "Tooling_Space"
        ["MSTest_Engine"]
        ["Moq_Library"]
        ["Coverlet_Collector"]
    end

    subgraph "Code_Entity_Space"
        ["BankAccountControllerTests"]
        ["IBankAccountService"]
        ["Test_Coverage_Reports"]
    end

    ["MSTest_Engine"] -- "Executes" --> ["BankAccountControllerTests"]
    ["Moq_Library"] -- "Mocks" --> ["IBankAccountService"]
    ["Coverlet_Collector"] -- "Generates" --> ["Test_Coverage_Reports"]
```
**Sources:** [tests/ApiUnitTests/ApiUnitTests.csproj:12-16]()

---