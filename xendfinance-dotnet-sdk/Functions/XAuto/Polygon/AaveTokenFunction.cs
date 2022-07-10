using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class AaveTokenFunction : AaveTokenFunctionBase
    { }

    [Function("aaveToken", "address")]
    public class AaveTokenFunctionBase : FunctionMessage
    {
    }
}