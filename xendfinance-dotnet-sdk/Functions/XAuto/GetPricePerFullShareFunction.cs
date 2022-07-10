using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class GetPricePerFullShareFunction : GetPricePerShareFunctionBase
    { }

    [Function("getPricePerFullShare", "uint256")]
    public class GetPricePerShareFunctionBase : FunctionMessage
    {
    }
}