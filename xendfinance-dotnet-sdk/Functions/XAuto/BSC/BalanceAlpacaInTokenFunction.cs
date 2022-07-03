using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceAlpacaInTokenFunction : BalanceAlpacaInTokenFunctionBase { }

    [Function("balanceAlpacaInToken", "uint256")]
    public class BalanceAlpacaInTokenFunctionBase : FunctionMessage
    {

    }
}
