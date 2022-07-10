using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceAlpacaFunction : BalanceAlpacaFunctionBase
    { }

    [Function("balanceAlpaca", "uint256")]
    public class BalanceAlpacaFunctionBase : FunctionMessage
    {
    }
}