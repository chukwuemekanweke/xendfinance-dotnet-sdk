﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions.XVault
{
    public partial class AvailableDepositLimitFunction : DepositLimitFunctionBase
    { }

    [Function("availableDepositLimit", "uint256")]
    public class DepositLimitFunctionBase : FunctionMessage
    {
    }
}