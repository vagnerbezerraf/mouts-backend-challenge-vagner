# Implement Sales API CRUD

This plan details the implementation of the Sales API according to the requirements specified in the Developer Evaluation project. The implementation will follow the existing Domain-Driven Design (DDD) and Clean Architecture patterns present in the `Ambev.DeveloperEvaluation` solution.

## User Review Required

- **Data Models**: The requirements ask for Customer, Branch, and Product. Since these domains might not exist or we only need to use "External Identities", I plan to use `CustomerId`, `BranchId`, and `ProductId` as `Guid` identifiers, along with denormalized names where appropriate, or just the IDs. Please confirm if this is acceptable.
- **Event Publishing**: I will implement domain events (`SaleCreatedEvent`, `SaleModifiedEvent`, `SaleCancelledEvent`, `ItemCancelledEvent`) and publish them using MediatR to a basic application log handler as specified ("it's not required to actually publish to any Message Broker").

## Open Questions

- Should we implement pagination/filtering for the "Read All" / "List" endpoint of the Sales API?
- Do we need to manage product stock or can we assume stock is always available for this evaluation?

## Proposed Changes

### Domain Layer
We will introduce the new entities, enums, validation, and domain events for Sales.

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Entities/Sale.cs`
- Represents the Sale aggregate root.
- Properties: `SaleNumber`, `SaleDate`, `CustomerId`, `BranchId`, `TotalAmount`, `IsCancelled`, and a collection of `SaleItem`.
- Methods: `CalculateTotal()`, `Cancel()`, `AddItem()`.

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Entities/SaleItem.cs`
- Represents an item in a Sale.
- Properties: `ProductId`, `Quantity`, `UnitPrice`, `Discount`, `TotalAmount`, `IsCancelled`.
- Methods: `Cancel()`.

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Events/SaleCreatedEvent.cs`
- Event triggered when a sale is created.
(And similar classes for `SaleModifiedEvent`, `SaleCancelledEvent`, `ItemCancelledEvent`).

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Repositories/ISaleRepository.cs`
- Interface for CRUD operations for Sales.

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Validation/SaleValidator.cs`
- FluentValidation rules for `Sale`.

#### [NEW] `Ambev.DeveloperEvaluation.Domain/Validation/SaleItemValidator.cs`
- Contains the business logic validation:
  - Cannot sell above 20 identical items.

---

### Application Layer
We will implement CQRS using MediatR for the Sales use cases.

#### [NEW] `Ambev.DeveloperEvaluation.Application/Sales/CreateSale/...`
- `CreateSaleCommand`, `CreateSaleResult`, `CreateSaleProfile`
- `CreateSaleHandler`: Contains logic to apply discounts (10% for >=4, 20% for >=10) and calculate totals before saving to the repository. Emits `SaleCreatedEvent`.

#### [NEW] `Ambev.DeveloperEvaluation.Application/Sales/UpdateSale/...`
- `UpdateSaleCommand`, `UpdateSaleResult`, `UpdateSaleProfile`
- `UpdateSaleHandler`: Updates sale, recalculates totals. Emits `SaleModifiedEvent`.

#### [NEW] `Ambev.DeveloperEvaluation.Application/Sales/GetSale/...`
- `GetSaleCommand`, `GetSaleResult`, `GetSaleProfile`
- `GetSaleHandler`: Retrieves a sale by ID.

#### [NEW] `Ambev.DeveloperEvaluation.Application/Sales/DeleteSale/...`
- `DeleteSaleCommand`, `DeleteSaleResponse`, `DeleteSaleProfile`
- `DeleteSaleHandler`: Cancels a sale (logical delete) and emits `SaleCancelledEvent`.

#### [NEW] `Ambev.DeveloperEvaluation.Application/Sales/EventHandlers/...`
- Handlers for the domain events that simply log the event to the application logger.

---

### ORM Layer
We will configure Entity Framework Core mappings for the new entities.

#### [NEW] `Ambev.DeveloperEvaluation.ORM/Mapping/SaleConfiguration.cs`
- EF Core mappings for `Sale` table.

#### [NEW] `Ambev.DeveloperEvaluation.ORM/Mapping/SaleItemConfiguration.cs`
- EF Core mappings for `SaleItem` table.

#### [MODIFY] `Ambev.DeveloperEvaluation.ORM/DefaultContext.cs`
- Add `DbSet<Sale>` and `DbSet<SaleItem>`.

#### [NEW] `Ambev.DeveloperEvaluation.ORM/Repositories/SaleRepository.cs`
- Implementation of `ISaleRepository`.

---

### WebApi Layer
We will create the REST endpoints.

#### [NEW] `Ambev.DeveloperEvaluation.WebApi/Features/Sales/SalesController.cs`
- Endpoints: `POST /api/sales`, `GET /api/sales/{id}`, `PUT /api/sales/{id}`, `DELETE /api/sales/{id}`.

#### [NEW] `Ambev.DeveloperEvaluation.WebApi/Features/Sales/CreateSale/...`
- `CreateSaleRequest`, `CreateSaleResponse`, `CreateSaleRequestValidator`, `CreateSaleProfile`
(And similar for Update, Get, Delete).

## Verification Plan

### Automated Tests
- Unit tests for `SaleItemValidator` to ensure the 20-item limit.
- Unit tests for `CreateSaleHandler` and `UpdateSaleHandler` to ensure discount rules (10% for 4-9 items, 20% for 10-20 items).
- Run existing unit tests to ensure nothing is broken.

### Manual Verification
- Run the WebApi application.
- Use Swagger UI to create a sale with different item quantities and verify the correct discounts and totals are applied.
- Verify in the logs that `SaleCreatedEvent` and `SaleCancelledEvent` are published.
