# 🌌 **Astronomic Catalogs**
This project is a demonstration of full-stack architectural and development skills using ASP.NET technologies. It is fully developed by me and is not intended for deployment on third-party systems, but rather as a portfolio-ready showcase of structured backend development, Identity customization, and advanced web UI integration with alerting, theming, and dynamic client scripting. 

✨ A live deployment is available here: https://newastrocatalogs.azurewebsites.net  

⚖️ The database is hosted on **Azure SQL Server**, and the codebase is managed on **Azure DevOps**, with **GitHub serving as a read-only mirror**:  
*   [Main repository *(active development)*](https://dev.azure.com/voldmytcOrganization/_git/Astronomic%20Catalogs/)
*   [Boards & Pipelines](https://dev.azure.com/voldmytcOrganization/Astronomic%20Catalogs/_dashboards/dashboard/afcb5290-0b24-4d15-b980-73f188b335be)
*   [Mirror on GitHub *(read-only)*](https://github.com/vdmytrk/Astronomic_Catalogs)

---

## 📑 **Table of Contents**
- [⚙️ Tech Stack](#⚙%EF%B8%8F-**tech-stack**)
- [📂 Project Structure Overview](#📂-**project-structure-overview**)
- [🏗️ Architecture & Design](#🏗%EF%B8%8F-**architecture-%26-design**)
- [🔐 Authentication & Authorization](#%F0%9F%94%90-**authentication-%26-authorization**)
  - [📧 Email Confirmation](#📧-**email-confirmation**)
- [🧠 Stored Procedure Highlights](#🧠-**stored-procedure-highlights**)
- [🧠 Key Stored Procedures & SQL Design](#🧠-**key-stored-procedures-%26-sql-design**)
- [🧪 Testing](#🧪-**testing**)
- [⚡ Developer Highlights](#⚡-**developer-highlights**)
- [🚨 Deployment Note](#🚨-**deployment-note**)
- [🛠️ Tooling](#🛠%EF%B8%8F-**tooling**)
- [🚧 Roadmap & Work in Progress](#🚧-**roadmap-%26-work-in-progress**)
- [📨 Contact](#📨-**contact**)

---

## ⚙️ **Tech Stack**
| **Area**             | **Technology / Approach**                                                    |
|----------------------|------------------------------------------------------------------------------|
| **Framework**        | ASP.NET Core 9 – MVC & Razor Pages                                           |
| **Frontend**         | TypeScript, LESS, Bootstrap 5, SweetAlert2, SignalR (real-time UI updates)   |
| **Bundling & Build** | Webpack, compilerconfig.json                                                 |
| **Database**         | Azure SQL Server + EF Core 9 (model-first + stored procedures + raw SQL)     |
| **Authentication**   | ASP.NET Identity + OAuth2 (Google, Microsoft), claims & role policies        |
| **Authorization**    | Policy-based, per-catalog & role-scoped UI and route protection              |
| **Email Delivery**   | SMTP (Gmail) + `ICustomEmailSender` with tokenized confirmation workflow     |
| **Logging**          | NLog (`NLog.config.<environment>`) + custom middleware                       |
| **Caching**          | `MemoryCache` with custom `ICacheService` (key tracking, expiration)         |
| **Rate Limiting**    | ASP.NET built-in limiter with TokenBucket, SlidingWindow, FixedWindow        |
| **DTO Handling**     | Manual mapping (AutoMapper demo included, not used in production)            |
| **Testing**          | Unit + Integration: NUnit, FakeItEasy, EF Core InMemory                      |
| **CI/CD**            | Azure Pipelines (`azure-pipelines.yml`)                                      |

⚡ **AutoMapper note**: While the project includes AutoMapper setup as a demo of prior experience, manual mapping is used in production due to licensing restrictions.  
⚡ **Mobile responsiveness** is partially implemented and remains incomplete at this stage.  

---

## 📂 **Project Structure Overview**
**Main Components**:
*   🌌 **Astronomic Catalogs** — ASP.NET MVC application with:
    *   EF Core (SQL + stored procedures)
    *   Authentication and Authorization
    *   Custom rate-limiting
    *   Custom middleware logging
    *   Catching
    *   UI theming with Bootstrap 5 and LESS (dark/light mode)
*   🔐 **Identity Admin Area** — Razor Pages + MVC for managing users, roles, and claims with consistent layout and UX patterns.
*   🧪 **Test Suite** — Testing in real HTTP scenarios using an isolated environment and test doubles.  

---

## 🏗️ **Architecture & Design**
*   **Stored Procedure Result Caching**:
    *   Leverages `IMemoryCache` to cache stored procedure results with a default expiration time.
    *   Logs cache hits and misses, and supports key-based or prefix-based invalidation.
*   **Custom Middleware Logging**:
    *   Logs every incoming HTTP request and error with metadata (status code, errors, browser, IP, referrer, route, etc.)
    *   Used environment-driven config.
    *   Errors routed to a central `ErrorController` with custom views for 403, 404, 500.
*   **Database Initialization**:
    *   The database is initialized — including creation of stored procedures, functions, and seed data — **only when explicitly enabled** by setting the following environment variable:
```json
        "InitializationDb": {
            "RestoreDatabase": true
        }
```
*   **Schema Design Strategy**:
    *   Model-first approach with raw SQL for tables not covered by models.
*   **Excel-Based Data Import**:
    *   Reading system data from preprocessed `.xlsx` files using `System.IO.Packaging` and OpenXml.
    *   Rows are processed in parallel with cancellation support and real-time progress reporting via SignalR.
*   **Rate Limiting**:
    *   Implemented using ASP.NET's built-in `AddRateLimiter` middleware.
    *   Custom policies for user/IP-based request throttling.
    *   Different strategies (FixedWindow, SlidingWindow, TokenBucket) applied to balance between registered and anonymous users.
*   **Frontend**:
    *   TypeScript compiled into `wwwroot/js` with modular client-side scripting.
    *   LESS compiled to CSS with runtime theme switching.
    *   SweetAlert2 alerts integrated via ES modules.
*   **Testing**:
    *   The tests are located in `ACTests.Tests`, contain unit tests that cover controller/service logic, as well as integration tests in isolation..
    *   Integration testing: full HTTP pipeline via `WebApplicationFactory`; verification of policies/roles/claims, admin panel navigation, and login scenarios through standard Identity UI pages.

---

## 🔐 **Authentication & Authorization**
This project supports both local and external login methods:
*   Local accounts via **ASP.NET Identity** (with cookie-based auth), includes email confirmation via Gmail SMTP with detailed logging of delivery attempts.
*   External logins via **OAuth2** (Google and Microsoft personal accounts) including claim mapping.

**Claims-** and **role-based authorization**, with **policy-based access control**.

### 📧 **Email Confirmation**
Email confirmation workflow for local account registration, featuring secure SMTP-based per-user delivery via a custom `ICustomEmailSender`, with built-in tracking, logging on failed sending, and a manual UI retry option.

Confirmation emails include:  
*   Secure verification link with a token.  
*   Retry metadata (`LastRegisterEmailSent`, `CountRegisterEmailSent`).

appsettings.json example:
```json
  "AuthMessageSenderOptions": {
    "Email": "YOUR-EMAIL-ADDRESS",
    "Password": "YOUR-EMAIL-PASSWORD"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR-GOOGLE-CLIENT-ID",
      "ClientSecret": "YOUR-GOOGLE-CLIENT-SECRET"
    },
    "Microsoft": {
      "ClientId": "YOUR-MICROSOFT-CLIENT-ID",
      "ClientSecret": "YOUR-MICROSOFT-CLIENT-SECRET"
    }
  },
  "JwtSettings": {
    "Key": "YOUR-JWT-SECRET-KEY",
    "Issuer": "YOUR-JWT-ISSUER",
    "Audience": "ACAuthClient",
    "ExpireMinutes": MINUTES
  }
```

---
       
## 🧠 **Stored Procedure Highlights**
This project uses SQL stored procedures as a core data transformation layer. They cover:
*   **ETL & Data Normalization** — Transform raw datasets into structured relational formats.
*   **Deduplication & Aggregation** — Select the most relevant values across grouped records using dynamic SQL.
*   **Flexible Filtering** — Dynamic filters with ranges, optional conditions, JSON arrays, and pagination.
*   **Transactional Safety** — Full use of transactions, rollback logic, and type-aware parsing.
*   **Logging & Exception Handling** — Detailed logging and exception handling with informative error messages and rethrowing for upstream visibility.

## 🧠 **Key Stored Procedures & SQL Design**      
*   🔄 ****Data Processing & Normalization****
    *   **CalculationPlanetarySystemData**: aggregates and transforms normalized catalog entries into structured relational data. Groups related records, extracts representative values, derives calculated fields by mathematical expressions, and populates boolean flags based on range conditions.
    *   **FillNASAExoplanetCatalogUniquePlanets**: generates a deduplicated dataset by selecting the most recent non-null value for each attribute across grouped entities. Dynamically builds a query to extract prioritized values per column, based on data type–specific filtering logic and latest date ordering. Ensures consistency across heterogeneous data types, applies fallback defaults, and generates the final dataset for insertion. 
    *   **InsertCollinderCatalog**: transforms raw catalog entries into a normalized structure. Handles parsing of positional data, extraction of numeric attributes, and general data cleanup to ensure consistency and integrity.
    *   **InsertNGCICOpendatasoft**: processes structured components from input strings, resolves object classifications, and assigns metadata such as the source origin.
    *   **MigrateNGCICOStoNGCICO_W**: performs a full migration and normalization. Detects duplicates and sub-objects, routes data accordingly, enriches records using data from supplemental tables.

*   🔍 ****Flexible Filtering Procedures****
    *   **GetFilteredNGCICData**, **GetFilteredPlanetarySystemsData**: apply dynamic filters using nullable conditions, ranges, and flexible matching logic.
    *   **GetFilteredPlanetsData**: supports advanced filtering with JSON arrays, range-based filters, and paginated output.

*   ⚙️ ****Performance & Extensibility****
    *   Leverages indexed queries and optimized conditions for scalability.
    *   Designed with parameterized filtering logic to ensure maintainability.
    *   Includes centralized logging for consistent error handling and monitoring.

---

## 🧪 **Testing**
The test infrastructure includes creating users with roles/claims, simulating UI logins, providing stable DI configuration, replacing external HTTP calls, and constructing flexible `ClaimsPrincipal` objects with configurable claims.

*   **Unit Tests**
    *   **Controllers** (`HomeController`, `HomeAdminController`): verify view returns, redirects, and model correctness.
    *   **Services** (`EmailSender`, caching, logging): validate business logic, error handling, and correct dependency calls.
    *   **Test doubles**: use mocks and fake repositories to isolate components.
*   **Integration infrastructure**
    *   **`TestAuthWebApplicationFactory`**: replaces authentication with `TestAuth`; manages user claims/roles via `TestAuthHandlerOptions` and `TestUserBuilder`.
    *   **`LoginViaUIWebApplicationFactory`**: runs full Identity UI on in-memory `ApplicationDbContext`.
*   **Isolation & Stability**
    *   *In-memory* database.
    *   Replacement or disabling of external services.
    *   Dedicated `Testing` environment and configuration overrides.
*   **Integration coverage**
    *   **Authorization by claims/roles**: expected `200/403` or redirects to login.
    *   **Identity UI**: failed login shows error; successful admin login allows access.
    *   **Admin Area / `UsersController`**: admin access and presence of key links; non-admin access blocked.

---

## ⚡ **Developer Highlights**
✅ Modular Program.cs architecture with factory-registered services, custom/conditional middleware.  
✅ Environment-driven configuration loading with support for per-environment settings and secure secrets from **Azure Key Vault**.  
✅ DTOs are used strategically in key areas to separate EF Core models from complex view logic and user-facing components.  
✅ Custom mapping between domain models, DTOs, and ViewModels with supports data flattening, conditional formatting, and grouping logic for complex UI rendering and output shaping.  
✅ SQL stored procedures power complex data processing — preferred over LINQ to leverage my experience in writing optimized SQL with CTEs, temp tables, and complex joins.  

---
      
## 🚨 **Deployment Note**
This solution **requires manual configuration and setup** to be operational. It is not ready for automatic deployment or immediate execution after cloning.

Key requirements:
*   Secrets, connection strings, and authentication keys.
*   SQL Server access and an initialized DB.
*   External script files for database seeding (not included in repo).

---

## 🛠️ **Tooling**
*   Visual Studio 2022
*   Azure DevOps (Repos, Boards, Pipelines)
*   SQL Server Management Studio (Azure SQL DB)
*   Git (GitHub & Azure DevOps)
*   Git Flow branching model

---

## 🚧 **Roadmap & Work In Progress**   
While this project is fully functional and complete in its core features, development is ongoing to refine and expand the project. Upcoming improvements include:
*   Two-factor authentication for admin access.
*   Broader test coverage; Scenario-based testing.
*   Enhanced mobile responsiveness.
*   Admin interface enhancements for bulk operations and better data management.
*   Support for additional catalogs and features.
*   UI/UX refinements.

---

## 📨 **Contact**
Feel free to reach out if you'd like to discuss architectural decisions, project structure, or deployment questions:
*   Email: [voldmytc@gmail.com](mailto:voldmytc@gmail.com)
*   DOU: [https://dou.ua/users/volodimir-dmiterko-1/?from=menu-profile](https://dou.ua/users/volodimir-dmiterko-1/?from=menu-profile)
*   GitHub: [https://github.com/vdmytrk](https://github.com/vdmytrk)
*   Azure: [Volodymyr Dmyterko](https://dev.azure.com/voldmytcOrganization/Profile/_wiki/wikis/Profile.wiki/2/About-me) 
