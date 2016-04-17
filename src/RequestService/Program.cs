using System.IO;
using log4net.Config;
using MassTransit.Log4NetIntegration.Logging;
using Topshelf;
using Topshelf.Logging;

namespace RequestService
{
    public class Program
    {
        public static int Main()
        {
            XmlConfigurator.Configure(new FileInfo("CommonLog4Net.config"));

            // Topshelf to use Log4Net
            Log4NetLogWriterFactory.Use();

            // MassTransit to use Log4Net
            Log4NetLogger.Use();

            return (int)HostFactory.Run(x => x.Service<RequestService>());
        }
    }
}