using System;
using System.Configuration;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Topshelf;
using Topshelf.Logging;
using MassTransit.ConsumeConfigurators;
using MassTransit.RabbitMqTransport.Configuration;

namespace RequestService
{
    public class RequestService : ServiceControl
    {
        readonly LogWriter _log = HostLogger.Get<RequestService>();

        IBusControl _busControl;

        public bool Start(HostControl hostControl)
        {
            _log.Info("Creating bus...");

            _busControl = Bus.Factory.CreateUsingRabbitMq(ConfigureRabbitMq);

            _log.Info("Starting bus...");

            _busControl.Start();

            return true;
        }

        private static void ConfigureRabbitMq(IRabbitMqBusFactoryConfigurator x)
        {
            IRabbitMqHost host = x.Host(new Uri(ConfigurationManager.AppSettings["RabbitMQHost"]), ConfigureHost);

            x.ReceiveEndpoint(host, ConfigurationManager.AppSettings["ServiceQueueName"], ConfigureEndPoint);
        }

        private static void ConfigureHost(IRabbitMqHostConfigurator hostConfig)
        {
        }

        private static void ConfigureEndPoint(IRabbitMqReceiveEndpointConfigurator endpointConfig)
        {
            endpointConfig.UseRetry(Retry.None);
            endpointConfig.Consumer<RequestConsumer>();// ConfigureEndpointConsumer);
        }

        private static void ConfigureEndpointConsumer(IConsumerConfigurator<RequestConsumer> consumeConfig)
        {
            consumeConfig.UseCircuitBreaker();
        }

        public bool Stop(HostControl hostControl)
        {
            _log.Info("Stopping bus...");

            _busControl?.Stop();

            return true;
        }
    }
}