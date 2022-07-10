using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XVault
{
    public partial class GetAPYFunction : GetAPYFunctionBase
    { }

    [Function("getAPYFunction", "uint256")]
    public class GetAPYFunctionBase : FunctionMessage
    {
    }
}