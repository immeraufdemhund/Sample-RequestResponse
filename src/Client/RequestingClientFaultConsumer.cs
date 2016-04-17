using MassTransit;
using System;
using System.Threading.Tasks;
using Sample.MessageTypes;

namespace Client
{
    class RequestingClientFaultConsumer : IConsumer<Fault<ICustomerInformationRequest>>
    {
        public async Task Consume(ConsumeContext<Fault<ICustomerInformationRequest>> context)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("OH NOES!!!");
            Console.ForegroundColor = color;
            await Task.FromResult(0);
        }
    }
}
