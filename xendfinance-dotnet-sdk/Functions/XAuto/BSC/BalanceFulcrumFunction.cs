using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xendfinance_dotnet_sdk.Functions.XAuto.BSC
{
    public partial class BalanceFulcrumFunction : BalanceFulcrumFunctionBase { }

    [Function("balanceFulcrum", "uint256")]
    public class BalanceFulcrumFunctionBase : FunctionMessage
    {

    }
}
