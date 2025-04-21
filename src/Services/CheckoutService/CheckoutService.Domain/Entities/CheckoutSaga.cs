using Stateless;

namespace CheckoutService.Domain.Entities;

public class CheckoutSaga
{
    // Parameterless constructor for EF
    protected CheckoutSaga()
    {
    }

    public CheckoutSaga(Guid orderId)
    {
        OrderId = orderId;
        State = SagaState.Pending;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = CreatedAt;
    }

    public Guid OrderId { get; private set; }
    public SagaState State { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime LastUpdatedAt { get; private set; }
    public string? LastError { get; private set; }

    /// <summary>
    ///     Transitions the saga to the next state based on the trigger.
    /// </summary>
    public void Transition(SagaTrigger trigger)
    {
        var stateMachine = new StateMachine<SagaState, SagaTrigger>(
            () => State,
            s => State = s);

        // Pending -> PaymentProcessing
        stateMachine.Configure(SagaState.Pending)
            .Permit(SagaTrigger.OrderCreated, SagaState.PaymentProcessing);

        // PaymentProcessing -> InventoryReserve or Compensated
        stateMachine.Configure(SagaState.PaymentProcessing)
            .Permit(SagaTrigger.PaymentSucceeded, SagaState.InventoryReserve)
            .Permit(SagaTrigger.PaymentFailed, SagaState.Compensated);

        // InventoryReserve -> InventoryReserved or Compensated
        stateMachine.Configure(SagaState.InventoryReserve)
            .Permit(SagaTrigger.InventoryReserved, SagaState.InventoryReserved)
            .Permit(SagaTrigger.InventoryFailed, SagaState.Compensated);

        // InventoryReserved -> Completed
        stateMachine.Configure(SagaState.InventoryReserved)
            .Permit(SagaTrigger.OrderCompleted, SagaState.Completed);

        // Fire trigger
        stateMachine.Fire(trigger);

        LastUpdatedAt = DateTime.UtcNow;
    }

    public void SetError(string error)
    {
        LastError = error;
        LastUpdatedAt = DateTime.UtcNow;
    }
}