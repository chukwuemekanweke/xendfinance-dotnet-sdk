using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceFortubeInTokenFunction : BalanceFortubeInTokenFunctionBase { }

    [Function("balanceFortubeInToken", "uint256")]
    public class BalanceFortubeInTokenFunctionBase : FunctionMessage
    {

    }
}
