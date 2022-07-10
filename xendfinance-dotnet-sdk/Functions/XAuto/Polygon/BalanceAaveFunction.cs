using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    public partial class BalanceAaveFunction : BalanceAaveBase
    { }

    [Function("balanceAave", "uint256")]
    public class BalanceAaveBase : FunctionMessage
    {
    }
}