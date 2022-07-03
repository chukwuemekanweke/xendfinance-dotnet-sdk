using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XAuto
{
    /// <summary>
    /// For retriving the balance of native tokens xBNB, xMatic
    /// </summary>
    public partial class BalanceFunction : BalanceFunctionBase { }

    [Function("token", "address")]
    public class BalanceFunctionBase : FunctionMessage
    {

    }
}
