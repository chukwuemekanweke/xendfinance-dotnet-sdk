using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.ServiceModels;

namespace xendfinance_dotnet_sdk.Interfaces
{
    internal interface IXVaultConnectorService
    {
        Task<string> DepositAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC, CancellationTokenSource? cancellationTokenSource = null);

        Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC, CancellationTokenSource? cancellationTokenSource = null);

        Task<string> WithdrawBySharesAsync(decimal shares, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC);

        Task<TransactionResponse> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC, CancellationTokenSource? cancellationTokenSource = null);

        Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC);

        Task<TransactionResponse> WithdrawAndWaitForReceiptAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC, CancellationTokenSource? cancellationTokenSource = null);

        Task<decimal> GetShareBalanceAsync(string address, Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetWorthOfSharesAsync(string address, Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetTotalAssetsAsync(Assets asset, Networks network = Networks.BSC);

        Task<decimal> MaxAvailableSharesAsync(Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetCreditAvailableAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetDebtOutstandingAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetPricePerShareAsync(Assets asset, Networks network = Networks.BSC);

        Task<decimal> GetDepositLimit(Assets asset, Networks network = Networks.BSC);

        Task<double> GetAPYAsync(Assets asset, Networks network = Networks.BSC);
    }
}