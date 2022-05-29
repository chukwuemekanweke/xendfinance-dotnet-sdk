using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.Shared
{
    [FunctionOutput]
    public class DecimalsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Decimals { get; set; }
    }

    public partial class DecimalsOutputDTO : DecimalsOutputDTOBase { }
}
