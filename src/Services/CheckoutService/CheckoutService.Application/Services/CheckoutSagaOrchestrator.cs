using System.Text.Json;
using BugBucks.Shared.Logging.Interfaces;
using BugBucks.Shared.Messaging.Contracts.DTOs;
using BugBucks.Shared.Messaging.Contracts.Events;
using CheckoutService.Domain.Entities;
using CheckoutService.Domain.Interfaces;
using CheckoutService.Infrastructure.Data;
using CheckoutService.Infrastructure.Entities;

namespace CheckoutService.Application.Services;

/// <summary>
///     Orchestrates checkout saga state transitions and enqueues events into the outbox.
/// </summary>
public class CheckoutSagaOrchestrator : ICheckoutSagaOrchestrator
{
    private readonly CheckoutOutboxDbContext _db;
    private readonly IAppLogger<CheckoutSagaOrchestrator> _logger;
    private readonly ICheckoutSagaRepository _repo;

    public CheckoutSagaOrchestrator(
        ICheckoutSagaRepository repo,
        CheckoutOutboxDbContext db,
        IAppLogger<CheckoutSagaOrchestrator> logger)
    {
        _repo = repo;
        _db = db;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);

        _logger.LogDebug("Handling {Type} for OrderId={OrderId}, state={State}",
            nameof(OrderCreatedEvent), evt.OrderId, saga.State);

        saga.Transition(SagaTrigger.OrderCreated);
        await _repo.SaveAsync(saga);

        // Enqueue PaymentRequestedEvent
        var paymentReq = new PaymentRequestedEvent(
            evt.OrderId,
            string.Empty,
            Guid.NewGuid().ToString());

        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateType = nameof(CheckoutSaga),
            AggregateId = evt.OrderId,
            Type = nameof(PaymentRequestedEvent),
            Content = JsonSerializer.Serialize(paymentReq),
            OccurredAt = DateTime.UtcNow
        };
        _db.OutboxMessages.Add(outbox);
        await _db.SaveChangesAsync();

        _logger.LogDebug("Saga {OrderId} transitioned to {NewState}", evt.OrderId, saga.State);
    }

    public Task HandleAsync(PaymentRequestedEvent evt)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(PaymentSucceededEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.PaymentSucceeded);
        await _repo.SaveAsync(saga);

        // Enqueue InventoryReserveRequestedEvent
        var invReq = new InventoryReserveRequestedEvent(evt.OrderId, Array.Empty<InventoryItem>());

        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateType = nameof(CheckoutSaga),
            AggregateId = evt.OrderId,
            Type = nameof(InventoryReserveRequestedEvent),
            Content = JsonSerializer.Serialize(invReq),
            OccurredAt = DateTime.UtcNow
        };
        _db.OutboxMessages.Add(outbox);
        await _db.SaveChangesAsync();
    }

    public async Task HandleAsync(PaymentFailedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.PaymentFailed);
        saga.SetError(evt.ErrorCode);
        await _repo.SaveAsync(saga);

        // Enqueue OrderCompensatedEvent
        var comp = new OrderCompensatedEvent(evt.OrderId, evt.ErrorCode);

        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateType = nameof(CheckoutSaga),
            AggregateId = evt.OrderId,
            Type = nameof(OrderCompensatedEvent),
            Content = JsonSerializer.Serialize(comp),
            OccurredAt = DateTime.UtcNow
        };
        _db.OutboxMessages.Add(outbox);
        await _db.SaveChangesAsync();
    }

    public Task HandleAsync(InventoryReserveRequestedEvent evt)
    {
        throw new NotImplementedException();
    }

    public async Task HandleAsync(InventoryReservedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.InventoryReserved);
        await _repo.SaveAsync(saga);

        // Enqueue OrderCompletedEvent
        var complete = new OrderCompletedEvent(evt.OrderId);

        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateType = nameof(CheckoutSaga),
            AggregateId = evt.OrderId,
            Type = nameof(OrderCompletedEvent),
            Content = JsonSerializer.Serialize(complete),
            OccurredAt = DateTime.UtcNow
        };
        _db.OutboxMessages.Add(outbox);
        await _db.SaveChangesAsync();
    }

    public async Task HandleAsync(InventoryFailedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.InventoryFailed);
        saga.SetError(evt.Reason);
        await _repo.SaveAsync(saga);

        // Enqueue OrderCompensatedEvent
        var comp = new OrderCompensatedEvent(evt.OrderId, evt.Reason);

        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            AggregateType = nameof(CheckoutSaga),
            AggregateId = evt.OrderId,
            Type = nameof(OrderCompensatedEvent),
            Content = JsonSerializer.Serialize(comp),
            OccurredAt = DateTime.UtcNow
        };
        _db.OutboxMessages.Add(outbox);
        await _db.SaveChangesAsync();
    }

    public async Task HandleAsync(OrderCompletedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.OrderCompleted);
        await _repo.SaveAsync(saga);
    }

    public async Task HandleAsync(OrderCompensatedEvent evt)
    {
        var saga = await _repo.LoadAsync(evt.OrderId);
        saga.Transition(SagaTrigger.OrderCompensated);
        saga.SetError(evt.Reason);
        await _repo.SaveAsync(saga);
    }
}