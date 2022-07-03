using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class GetPricePerFullShareOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Value { get; set; }
    }

    public partial class GetPricePerFullShareOutputDTO : GetPricePerFullShareOutputDTOBase { }
}
