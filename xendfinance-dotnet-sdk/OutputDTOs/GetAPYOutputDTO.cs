using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs
{
    [FunctionOutput]
    public class GetAPYOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger APY { get; set; }
    }

    public partial class GetAPYOutputDTO : GetAPYOutputDTOBase { }
}
