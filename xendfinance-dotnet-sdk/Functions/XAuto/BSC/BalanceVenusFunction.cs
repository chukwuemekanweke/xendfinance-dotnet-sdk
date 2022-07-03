using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceVenusFunction : BalanceVenusFunctionBase { }

    [Function("balanceVenus", "uint256")]
    public class BalanceVenusFunctionBase : FunctionMessage
    {

    }
}
