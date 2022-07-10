using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class FortubeTokenFunction : FortubeTokenFunctionBase
    { }

    [Function("fortubeToken", "address")]
    public class FortubeTokenFunctionBase : FunctionMessage
    {
    }
}