using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class BalanceFortubeFunction : BalanceFortubeBase { }

    [Function("balanceFortube", "uint256")]
    public class BalanceFortubeBase : FunctionMessage
    {

    }
}
