namespace xendfinance_dotnet_sdk.Interfaces
{
    internal interface IXVaultConnectorService
    {
        Task DepositAsync(decimal amount);
        Task WithdrawBySharesAsync(decimal shares);
        Task WithdrawAsync(decimal amount);
        Task ShareBalanceAsync(string address);
        Task MaxAvailableSharesAsync();
        Task<double> GetAPYAsync();

    }
}
