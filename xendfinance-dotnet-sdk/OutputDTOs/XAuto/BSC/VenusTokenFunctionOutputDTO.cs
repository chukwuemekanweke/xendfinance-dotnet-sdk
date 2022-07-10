using Nethereum.ABI.FunctionEncoding.Attributes;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class VenusTokenFunctionOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string Token { get; set; } = string.Empty;
    }

    public partial class VenusTokenFunctionOutputDTO : VenusTokenFunctionOutputDTOBase
    { }
}