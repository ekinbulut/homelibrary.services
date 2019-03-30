using System.Threading.Tasks;
using Library.Common.Contracts.Events.BookEvents;
using MassTransit;

namespace Library.Common.Contracts.Events.BookEvents
{
    public class BookCreatedEventConsumer : IConsumer<BookCreated>
    {
        public Task Consume(ConsumeContext<BookCreated> context)
        {
            //TODO : fill database    
            return Task.CompletedTask;
        }
    }
}