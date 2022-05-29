using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs
{
    [FunctionOutput]
    public class TotalAssetsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger TotalAssets { get; set; }
    }

    public partial class TotalAssetsOutputDTO : TotalAssetsOutputDTOBase { }
}
