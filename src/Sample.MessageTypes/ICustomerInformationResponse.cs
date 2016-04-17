using System;

namespace Sample.MessageTypes
{
    public interface ICustomerInformationResponse
    {
        string CusomerName { get; }
        DateTime BirthDay { get; }
        decimal AnnualIncome { get; }
    }
}