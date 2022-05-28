using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.ServiceModels;

namespace xendfinance_dotnet_sdk.Interfaces
{
    internal interface IGasEstimatorService
    {
        Task<GasEstimateResponse> EstimateGas(Networks network);
    }
}