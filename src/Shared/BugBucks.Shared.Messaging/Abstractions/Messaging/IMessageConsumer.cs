using System;
using System.Threading.Tasks;

namespace BugBucks.Shared.Messaging.Abstractions.Messaging
{
    public interface IMessageConsumer
    {
        Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
    }
}
