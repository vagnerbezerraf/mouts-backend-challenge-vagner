# Sales API Implementation Walkthrough

The Sales CRUD API has been fully implemented following the Developer Evaluation guidelines. Here is a breakdown of what was achieved:

## 1. Domain Layer
- **Entities**: Created `Sale` and `SaleItem` mimicking the clean architecture already established in the solution. Both inherit from `BaseEntity`.
- **Validation**: Added `SaleValidator` and `SaleItemValidator` to validate simple structures and the 20-item maximum limit.
- **Events**: Implemented `SaleCreatedEvent`, `SaleModifiedEvent`, `SaleCancelledEvent`, and `ItemCancelledEvent` applying the `INotification` interface from MediatR to allow robust CQRS pipelines.

## 2. Application Layer (CQRS)
- **Commands & Queries**: Built out CQRS structure for `CreateSale`, `UpdateSale`, `GetSale`, and `DeleteSale`. Each operation has its dedicated Request, Handler, Command/Query, Result, Validator, and AutoMapper Profile.
- **Business Rules**: Inside `CreateSaleHandler` and `UpdateSaleHandler`, the discount tiers were implemented directly:
  - `Quantity >= 4` and `< 10`: Applies 10% discount.
  - `Quantity >= 10` and `<= 20`: Applies 20% discount.
  - Above 20 is already halted by the command validator.
- **Event Logging**: Added `SaleEventHandlers` class to intercept MediatR `INotification` requests and record actions using `Microsoft.Extensions.Logging` instead of an actual message broker, saving infrastructure overhead for this evaluation.

## 3. ORM Layer
- Added `SaleConfiguration` and `SaleItemConfiguration` using Fluent API inside Entity Framework Core.
- Registered the new DbSets within `DefaultContext`.
- Realized `SaleRepository` completing the `ISaleRepository` interface with `CreateAsync`, `GetByIdAsync`, `UpdateAsync`, and `DeleteAsync`.

## 4. WebApi Layer
- Orchestrated `SalesController` following RESTful practices using `[HttpPost]`, `[HttpGet("{id}")]`, `[HttpPut("{id}")]`, and `[HttpDelete("{id}")]` connected strictly to our CQRS handlers.

## 5. Verification
- We verified the build configuration via `dotnet build`.
- Added unit tests directly validating the core pricing logic for 4 and 10 item rules (`CreateSaleHandlerTests`).
- Successfully ran `dotnet test`, completing 51 valid tests.
