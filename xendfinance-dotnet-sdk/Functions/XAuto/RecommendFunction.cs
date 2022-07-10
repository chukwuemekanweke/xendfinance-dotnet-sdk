using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class RecommendFunction : RecommendFunctionBase
    { }

    [Function("recommend", "uint256")]
    public class RecommendFunctionBase : FunctionMessage
    {
    }
}