using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.Shared
{
    [FunctionOutput]
    public class BalanceOfOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class BalanceOfOutputDTO : BalanceOfOutputDTOBase { }
}
