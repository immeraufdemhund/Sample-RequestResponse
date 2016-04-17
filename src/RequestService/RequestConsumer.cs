using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Logging;
using Sample.MessageTypes;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace RequestService
{
    public class RequestConsumer : IConsumer<ICustomerInformationRequest>
    {
        readonly ILog _log = Logger.Get<RequestConsumer>();

        public async Task Consume(ConsumeContext<ICustomerInformationRequest> context)
        {
            _log.InfoFormat("Returning name for {0}", context.Message.CustomerId);
            var customer = await GetCustomer(context.Message.CustomerId);
            if (customer == null)
                throw new Exception("ITS NULL YOU MORON");
            //await context.NotifyFaulted(context, TimeSpan.FromSeconds(1), typeof(ICustomerInformationRequest).Name, new ArgumentException(context.Message.CustomerId + " returned 0 results"));
            else
                context.Respond(customer);
        }

        private static async Task<SimpleResponse> GetCustomer(string customerKey)
        {
            var adventureWorksConfig = ConfigurationManager.ConnectionStrings["AdventureWorksWarehouse"];
            string sql = @"
select
    c.CustomerAlternateKey,
    c.FirstName,
    c.LastName,
    c.BirthDate,
    c.YearlyIncome
from
    DimCustomer c (nolock)
where
    c.CustomerAlternateKey = @customerKey";
            var factory = DbProviderFactories.GetFactory(adventureWorksConfig.ProviderName);
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = adventureWorksConfig.ConnectionString;
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;

                    AddParameter(command, "@customerKey", customerKey);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return null;

                        return ParseResponseFromRecord(reader);
                    }
                }
            }
        }
        private static SimpleResponse ParseResponseFromRecord(IDataRecord record)
        {
            return new SimpleResponse
            {
                CusomerName = $"{record["FirstName"]} {record["LastName"]}",
                BirthDay = (DateTime)record["BirthDate"],
                AnnualIncome = (decimal)record["YearlyIncome"]
            };
        }
        private static void AddParameter(DbCommand command, string parameterName, string customerKey)
        {
            var param = command.CreateParameter();
            param.ParameterName = parameterName;
            param.Value = customerKey;

            command.Parameters.Add(param);
        }

        public class SimpleResponse : ICustomerInformationResponse
        {
            public decimal AnnualIncome { get; set; }

            public DateTime BirthDay { get; set; }

            public string CusomerName { get; set; }
        }
    }
}