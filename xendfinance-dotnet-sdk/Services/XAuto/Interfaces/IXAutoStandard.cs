using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.ServiceModels;

namespace xendfinance_dotnet_sdk.Services.XAuto.Interfaces
{
    public interface IXAutoStandard
    {
        Task<decimal> BalanceFortube(Assets asset);

        Task<decimal> BalanceFortubeInToken(Assets asset);

        Task<decimal> BalanceFulcrum(Assets asset);

        Task<decimal> BalanceFulcrumInToken(Assets asset);

        Task<decimal> CalculatePoolValueInToken(Assets asset);

        Task<string> FeeAddress(Assets asset);

        Task<decimal> FeeAmount(Assets asset);

        Task<decimal> GetPricePerFullShareAsync(Assets asset);

        Task<decimal> GetShareBalanceAsync(string address, Assets asset);

        Task<decimal> GetWorthOfSharesAsync(string address, Assets asset);

        Task<string> Token(Assets asset);

        Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource);

        Task<string> DepositAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource);

        Task<TransactionResponse> WithdrawAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource);

        Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset);

        Task<TransactionResponse> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource);

        Task<string> WithdrawBySharesAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset);
    }
}