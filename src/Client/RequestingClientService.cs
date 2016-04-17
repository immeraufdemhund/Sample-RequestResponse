using MassTransit;
using Sample.MessageTypes;
using System;
using System.Threading.Tasks;

namespace Client
{
    internal class RequestingClientService : IDisposable
    {
        private bool disposedValue;

        private readonly IBusControl _bus;
        private readonly string _serviceAddress;
        private IRequestClient<ICustomerInformationRequest, ICustomerInformationResponse> _client;

        public RequestingClientService(IBusControl bus, string serviceAddress)
        {
            _bus = bus;
            _serviceAddress = serviceAddress;
        }
 
        public void Start()
        {
            _bus.Start();
            CreateRequestClient();
        }

        public async Task<ICustomerInformationResponse> ReceiveClientRequest(string customerId)
        {
            return await _client.Request(new SimpleRequest(customerId));
        }

        private void CreateRequestClient()
        {
            _client = _bus.CreateRequestClient<ICustomerInformationRequest, ICustomerInformationResponse>(new Uri(_serviceAddress), TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _bus.Stop();
                }
                disposedValue = true;
            }
        }
    }
}
