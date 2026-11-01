# ERP Backlog

## Inventory: StockMovement domain
- [ ] Remove or validate the unused `type` parameter in `RecordOut`
- [ ] Fix copy-pasted error message in `RecordOut` ("received" -> "issued")
- [ ] Make `StockMovement.Build` private
- [ ] Validate `type` in `RecordIn` (inbound types only)
- [ ] Replace `balanceAfter` parameter with `currentBalance` (entity calculates the balance)
- [ ] Add `Reverse(...)` method and `ReversesMovementId` property
- [ ] Add `Initial` and `Reversal` to `StockMovementType`
- [ ] Create EF migration for the new column(s)
- [ ] Add concurrency protection (`RowVersion` or serializable transaction) for balance updates

## Inventory: Application layer
- [ ] Move `AdjustStock` to `Inventory/StockMovements/Commands/`
- [ ] Add `StockLevelDto` in `Inventory/StockMovements/`
- [ ] Add `GetStockLevel` query in `Inventory/StockMovements/Queries/`
- [ ] Add `GetStockMovements` (history) query
- [ ] Add `ReceiveStock`, `IssueStock`, `ReverseMovement` commands
- [ ] Optional: `GetLowStockProducts` query
- [ ] Fix `CreateProductCommandHandler`: call `RecordIn` with `StockMovementType.Initial`

## API
- [ ] Make sure `ProductsController` has a `GetById(Guid id)` action (needed by `CreatedAtAction`)
- [ ] Enable `JsonStringEnumConverter` so enums accept names in Swagger

## Identity / Auth
- [ ] Add `Users` and `RefreshTokens` DbSets in `AppDbContext`
- [ ] Register `IUserRepository` / refresh token repository in Infrastructure `DependencyInjection.cs`
- [ ] Create `IPasswordHasher` in `Erp.Application/Common/Interfaces/`
- [ ] Implement `PasswordHasher` in Infrastructure and register it
- [ ] Add login, register, and refresh token commands
- [ ] Configure EF navigation access mode for `RefreshTokens` backing field if needed

## Sales / Orders: known risks
- [ ] Order number race: two orders placed at the same instant can get the same number; the unique index rejects one with a 500. Retry on failure or use a database sequence.
- [ ] Overselling race: two people ordering the last units at the same moment can both pass the stock check. Add RowVersion concurrency on the product's stock.
