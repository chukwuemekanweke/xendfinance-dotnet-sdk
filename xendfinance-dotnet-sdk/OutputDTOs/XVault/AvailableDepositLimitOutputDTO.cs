using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XVault
{
    [FunctionOutput]
    public class AvailableDepositLimitOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger DepositLimit { get; set; }
    }

    public partial class AvailableDepositLimitOutputDTO : AvailableDepositLimitOutputDTOBase
    { }
}