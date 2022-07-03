using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class CalculatePoolValueInTokenFunction : CalculatePoolValueInTokenFunctionBase { }

    [Function("calcPoolValueInToken", "uint256")]
    public class CalculatePoolValueInTokenFunctionBase : FunctionMessage
    {

    }
}
