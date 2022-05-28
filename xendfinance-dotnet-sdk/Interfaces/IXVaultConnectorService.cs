using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.Interfaces
{
    internal interface IXVaultConnectorService
    {
        Task<string> DepositAsync(decimal amount, Assets asset, Networks network=Networks.BSC);
        Task<string> DepositAndWaitForReceiptAsync(decimal amount, Assets asset, Networks network = Networks.BSC);
        Task<string> WithdrawBySharesAsync(decimal shares, Assets asset, Networks network = Networks.BSC);
        Task<string> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, Assets asset, Networks network = Networks.BSC);
        Task WithdrawAsync(decimal amount, Assets asset, Networks network = Networks.BSC);
        Task WithdrawAndWaitForReceiptAsync(decimal amount, Assets asset, Networks network = Networks.BSC);
        Task<decimal> ShareBalanceAsync(string address, Assets asset, Networks network = Networks.BSC);
        Task<decimal> MaxAvailableSharesAsync(Assets asset, Networks network = Networks.BSC);
        Task<decimal> GetCreditAvailableAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC);
        Task<decimal> GetDebtOutstandingAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC);
        Task<decimal> GetPricePerShareAsync(Assets asset, Networks network = Networks.BSC);
        Task<decimal> GetDepositLimit(Assets asset, Networks network = Networks.BSC);
        Task<double> GetAPYAsync(Assets asset, Networks network = Networks.BSC);
    }
}
