using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class RecommendFunctionOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Lender { get; set; }
    }

    public partial class RecommendFunctionOutputDTO : RecommendFunctionOutputDTOBase { }
}
