using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class BalanceFortubeInTokenOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class BalanceFortubeInTokenOutputDTO : BalanceFortubeInTokenOutputDTOBase
    { }
}