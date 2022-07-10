using Nethereum.ABI.FunctionEncoding.Attributes;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class FeeAddressOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string Address { get; set; } = string.Empty;
    }

    public partial class FeeAddressOutputDTO : FeeAddressOutputDTOBase
    { }
}