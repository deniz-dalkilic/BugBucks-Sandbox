using System;
using System.Threading.Tasks;
using BugBucks.Shared.Messaging.Events;
using BugBucks.Shared.Messaging.Services;
using CheckoutService.Domain.Entities;
using CheckoutService.Infrastructure.Repositories;

namespace CheckoutService.Application.Services
{
    /// <summary>
    /// Orchestrates checkout saga state transitions and event publication.
    /// </summary>
    public class CheckoutSagaOrchestrator : ICheckoutSagaOrchestrator
    {
        private readonly ICheckoutSagaRepository _repository;
        private readonly CheckoutSagaPublisher _publisher;

        public CheckoutSagaOrchestrator(
            ICheckoutSagaRepository repository,
            CheckoutSagaPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task HandleAsync(OrderCreatedEvent evt)
        {
            // Just create saga and set state to PaymentProcessing
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.OrderCreated);
            await _repository.SaveAsync(saga);
            // Note: PaymentRequestedEvent is published by API endpoint
        }

        public async Task HandleAsync(PaymentSucceededEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.PaymentSucceeded);
            await _repository.SaveAsync(saga);

            // TODO: fetch order items for inventory reservation
            await _publisher.PublishInventoryReserveRequestedAsync(evt.OrderId, Array.Empty<InventoryItem>());
        }

        public async Task HandleAsync(PaymentFailedEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.PaymentFailed);
            saga.SetError(evt.ErrorCode);
            await _repository.SaveAsync(saga);

            // Compensate saga
            await _publisher.PublishOrderCompensatedAsync(evt.OrderId, evt.ErrorCode);
        }

        public async Task HandleAsync(InventoryReservedEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.InventoryReserved);
            await _repository.SaveAsync(saga);

            // Complete order
            await _publisher.PublishOrderCompletedAsync(evt.OrderId);
        }

        public async Task HandleAsync(InventoryFailedEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.InventoryFailed);
            saga.SetError(evt.Reason);
            await _repository.SaveAsync(saga);

            // Compensate saga
            await _publisher.PublishOrderCompensatedAsync(evt.OrderId, evt.Reason);
        }

        public async Task HandleAsync(OrderCompletedEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.OrderCompleted);
            await _repository.SaveAsync(saga);
        }

        public async Task HandleAsync(OrderCompensatedEvent evt)
        {
            var saga = await _repository.LoadAsync(evt.OrderId);
            saga.Transition(SagaTrigger.OrderCompensated);
            saga.SetError(evt.Reason);
            await _repository.SaveAsync(saga);
        }
    }
}