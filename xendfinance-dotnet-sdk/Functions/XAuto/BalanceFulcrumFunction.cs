using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class BalanceFulcrumFunction : BalanceFulcrumFunctionBase
    { }

    [Function("balanceFulcrum", "uint256")]
    public class BalanceFulcrumFunctionBase : FunctionMessage
    {
    }
}