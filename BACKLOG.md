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

- Concurrency on Order: two simultaneous cancels could return stock twice (RowVersion)
- Order status history (who changed the status, and when)

- User search by email, and deactivating a user account from the API
- Prevent demoting the last remaining Admin

- Order number race (two orders at the same instant)
- Order status history (who changed the status, and when)
- User search by email, and deactivating a user account from the API
- Prevent demoting the last remaining Admin

- Include a trace id in 500 responses so users can quote it to support
- Startup admin seeder fails the whole app when the database is down

- Forwarded headers so rate limiting sees the real client IP behind a proxy or Docker
- Per-account lockout after repeated failed logins (IP limits do not stop attackers spread over many IPs)
- Shared rate limit store (for example Redis) if the API runs on more than one instance
- Sliding window limiter instead of fixed window

- Order confirmation email through a domain event (email step 2)
- Per-address limit on resend-confirmation so one victim cannot be flooded with emails
- Password reset by email
- Background queue for sending emails instead of sending inside the request

- Outbox pattern: store events in the database in the same transaction, and send from a background worker so a crash cannot lose an email
- Emails for order confirmation, shipping, delivery, and cancellation (handlers on new events)
- HTML email templates, instead of building the markup in code
- Retry with backoff when sending an email fails