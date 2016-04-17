using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using log4net.Config;
using MassTransit;
using MassTransit.Log4NetIntegration.Logging;
using Sample.MessageTypes;
using ConsoleEngine;
using MassTransit.RabbitMqTransport.Configuration;

namespace Client
{
    public class Program
    {
        public static void Main()
        {
            XmlConfigurator.Configure(new FileInfo("CommonLog4Net.config"));

            // MassTransit to use Log4Net
            Log4NetLogger.Use();
            using (var service = new RequestingClientService(CreateBus(), ConfigurationManager.AppSettings["ServiceAddress"]))
            {
                var consoleEngineArguments = new ConsoleEngineArguments<ICustomerInformationResponse>
                {
                    OptionalHeaderToShowOnEachLoop = "Enter Customer id:",
                    ResultFromUserInput = service.ReceiveClientRequest,
                    UseNonNullResponse = ShowCustomerInformation
                };

                service.Start();
                Task.Run(async ()=> 
                {
                    await LoopWhile.NotQuit(consoleEngineArguments);
                }).Wait();
            }
        }
        private static void ShowCustomerInformation(ICustomerInformationResponse response)
        {
            Console.WriteLine("{0}", response.CusomerName);
            Console.WriteLine("    AnnualIncome:{0}", response.AnnualIncome);
            Console.WriteLine("    BirthDay:{0}", response.BirthDay.ToLongDateString());
        }
        static IBusControl CreateBus()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(ConfigureRabbitMq);
            return bus;
        }

        private static void ConfigureRabbitMq(IRabbitMqBusFactoryConfigurator rabbitMq)
        {
            var uri = new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]);
            var host = rabbitMq.Host(uri, ConfigureHost);
            rabbitMq.ReceiveEndpoint(host, endpointConfigure =>
            {
                endpointConfigure.Consumer<RequestingClientFaultConsumer>();
            });
        }

        private static void ConfigureHost(IRabbitMqHostConfigurator h)
        {
        }
    }
}