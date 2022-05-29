using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XVault
{
    [FunctionOutput]
    public class MaxAvailableSharesOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger MaxAvailableShares { get; set; }
    }

    public partial class MaxAvailableSharesOutputDTO : MaxAvailableSharesOutputDTOBase { }
}
