using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class FeeAmountFunction : FeeAmountFunctionBase { }

    [Function("feeAmount", "uint256")]
    public class FeeAmountFunctionBase : FunctionMessage
    {

    }
}
