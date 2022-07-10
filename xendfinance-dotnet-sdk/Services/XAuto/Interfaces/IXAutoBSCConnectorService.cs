using System.Numerics;
using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.Services.XAuto.Interfaces
{
    public interface IXAutoBSCConnectorService : IXAutoStandard
    {
        Task<BigInteger> BalanceAlpaca(Assets asset);

        Task<BigInteger> BalanceAlpacaInToken(Assets asset);

        Task<decimal> BalanceVenus(Assets asset);

        Task<decimal> BalanceVenusInToken(Assets asset);

        Task<LenderBSC> Recommend(Assets asset);

        Task<string> VenusToken(Assets asset, Networks networks);
    }
}