namespace xendfinance_dotnet_sdk.Models.ServiceModels
{
    public class TransactionResponse
    {
        public bool IsSuccessful { get; set; }
        public string BlockHash { get; set; }
        public string TransactionHash { get; set; }
    }
}