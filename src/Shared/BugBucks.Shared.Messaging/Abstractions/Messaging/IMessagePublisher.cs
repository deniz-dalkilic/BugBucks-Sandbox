using System.Threading.Tasks;

namespace BugBucks.Shared.Messaging.Abstractions.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}
