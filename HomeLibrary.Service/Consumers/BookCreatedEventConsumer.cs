using System.Threading.Tasks;
using Library.Common.Contracts.Events.BookEvents;
using MassTransit;

namespace HomeLibrary.Service.Consumers
{
    public class BookCreatedEventConsumer : IConsumer<BookCreated>
    {
        public Task Consume(ConsumeContext<BookCreated> context)
        {
            throw new System.NotImplementedException();
        }
    }
}