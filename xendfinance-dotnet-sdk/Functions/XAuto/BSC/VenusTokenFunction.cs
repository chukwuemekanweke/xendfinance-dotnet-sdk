using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class VenusTokenFunction : VenusTokenFunctionBase
    { }

    [Function("venusToken", "address")]
    public class VenusTokenFunctionBase : FunctionMessage
    {
    }
}