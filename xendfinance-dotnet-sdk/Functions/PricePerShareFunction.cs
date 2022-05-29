﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions
{
    public partial class PricePerShareFunction : PricePerShareFunctionBase { }

    [Function("pricePerShare", "uint256")]
    public class PricePerShareFunctionBase : FunctionMessage
    {
       
    }
}
