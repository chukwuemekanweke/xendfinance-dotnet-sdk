﻿using Nethereum.ABI.FunctionEncoding.Attributes;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto
{
    [FunctionOutput]
    public class FortubeTokenFunctionOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("address", "", 1)]
        public virtual string Token { get; set; } = string.Empty;
    }

    public partial class FortubeTokenFunctionOutputDTO : FortubeTokenFunctionOutputDTOBase
    { }
}