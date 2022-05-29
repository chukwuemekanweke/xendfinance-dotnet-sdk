using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions
{
    public partial class DecimalsFunction : DecimalsFunctionBase { }

    [Function("decimals", "uint256")]
    public class DecimalsFunctionBase : FunctionMessage
    {

    }
}
