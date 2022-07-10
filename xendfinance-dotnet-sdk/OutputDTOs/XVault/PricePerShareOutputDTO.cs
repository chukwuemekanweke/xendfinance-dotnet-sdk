using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XVault
{
    [FunctionOutput]
    public class PricePerShareOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger PricePerShare { get; set; }
    }

    public partial class PricePerShareOutputDTO : PricePerShareOutputDTOBase
    { }
}