using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace xendfinance_dotnet_sdk.Functions
{
    public partial class TotalAssetsFunction : TotalAssetsFunctionBase { }

    [Function("totalAssets", "uint256")]
    public class TotalAssetsFunctionBase : FunctionMessage
    {
    
    }
}
