using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class FeeAddressFunction : FeeAddressFunctionBase { }

    [Function("feeAddress", "address")]
    public class FeeAddressFunctionBase : FunctionMessage
    {

    }
}
