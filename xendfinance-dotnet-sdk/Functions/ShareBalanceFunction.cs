using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions
{
    public partial class ShareBalanceFunction : ShareBalanceInputFunctionBase { }

    [Function("balanceOf", "uint256")]
    public class ShareBalanceInputFunctionBase : FunctionMessage
    {
        [Parameter("address", "account", 1)]
        public virtual string Account { get; set; }
    }
}