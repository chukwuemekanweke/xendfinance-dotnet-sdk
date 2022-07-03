using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceVenusInTokenFunction : BalanceVenusInTokenFunctionBase { }

    [Function("balanceFulcrumInToken", "uint256")]
    public class BalanceVenusInTokenFunctionBase : FunctionMessage
    {

    }
}
