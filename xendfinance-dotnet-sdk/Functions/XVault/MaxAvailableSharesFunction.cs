using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XVault
{
    public partial class MaxAvailableSharesFunction : MaxAvailableSharesFunctionBase
    { }

    [Function("maxAvailableShares", "uint256")]
    public class MaxAvailableSharesFunctionBase : FunctionMessage
    {
    }
}