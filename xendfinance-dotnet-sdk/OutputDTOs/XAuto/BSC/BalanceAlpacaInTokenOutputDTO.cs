﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;

namespace xendfinance_dotnet_sdk.OutputDTOs.XAuto.BSC
{
    [FunctionOutput]
    public class BalanceAlpacaInTokenOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger Balance { get; set; }
    }

    public partial class BalanceAlpacaInTokenOutputDTO : BalanceAlpacaInTokenOutputDTOBase { }
}
