using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.Services.XAuto.Interfaces
{
    public interface IXAutoPolygonConnectorService : IXAutoStandard
    {
        Task<string> AaveToken(Assets asset, Networks networks);

        Task<decimal> BalanceAave(Assets asset, Networks networks);

        Task<LenderPolygon> Recommend(Assets asset);
    }
}