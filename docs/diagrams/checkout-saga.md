# Checkout Saga State Machine

State definitions:

- **Pending**: Initial state after order is created.
- **PaymentProcessing**: Waiting for payment result.
- **PaymentSucceeded**: Payment completed successfully.
- **PaymentFailed**: Payment was declined or errored.
- **InventoryReserve**: Requesting reservation of items.
- **InventoryReserved**: Inventory successfully reserved.
- **InventoryFailed**: Inventory reservation failed.
- **Completed**: Full checkout flow completed.
- **Compensated**: Rollback/compensation path executed.

```mermaid
stateDiagram-v2
  [*] --> Pending
  Pending --> PaymentProcessing : OrderCreatedEvent
  PaymentProcessing --> PaymentSucceeded : PaymentSucceededEvent
  PaymentProcessing --> PaymentFailed : PaymentFailedEvent
  PaymentFailed --> Compensated : CompensationHandler
  PaymentSucceeded --> InventoryReserve : InventoryReserveRequestedEvent
  InventoryReserve --> InventoryReserved : InventoryReservedEvent
  InventoryReserve --> InventoryFailed : InventoryFailedEvent
  InventoryReserved --> Completed : OrderCompletedEvent
  InventoryFailed --> Compensated : CompensationHandler
  Compensated --> [*]
  Completed --> [*]
