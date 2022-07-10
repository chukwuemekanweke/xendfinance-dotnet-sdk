namespace xendfinance_dotnet_sdk.Models.ServiceModels
{
    internal class GasEstimateResponse
    {
        public double FastGas { get; set; }
        public double AverageGas { get; set; }
        public double LowGas { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    #region BSC Gas API Response

    internal class BSCGasEstimateResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public BSCResult Result { get; set; }
    }

    internal class BSCResult
    {
        public ulong LastBlock { get; set; }
        public double SafeGasPrice { get; set; }
        public double ProposeGasPrice { get; set; }
        public double FastGasPrice { get; set; }
        public decimal UsdPrice { get; set; }
    }

    #endregion BSC Gas API Response

    #region Polygon Gas API Response

    internal class PolygonGasEstimateResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public PolygonResult Result { get; set; }
    }

    internal class PolygonResult
    {
        public ulong LastBlock { get; set; }
        public double SafeGasPrice { get; set; }
        public double ProposeGasPrice { get; set; }
        public double FastGasPrice { get; set; }
        public decimal BaseFee { get; set; }
        public decimal UsdPrice { get; set; }
    }

    #endregion Polygon Gas API Response

    #region Ethereum Gas API Response

    internal class EthereumGasEstimateResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public EthereumResult Result { get; set; }
    }

    internal class EthereumResult
    {
        public ulong LastBlock { get; set; }
        public double SafeGasPrice { get; set; }
        public double ProposeGasPrice { get; set; }
        public double FastGasPrice { get; set; }
        public decimal SuggestBaseFee { get; set; }
    }

    #endregion Ethereum Gas API Response

    #region HecoChain Gas API Response

    internal class HecoChainGasEstimateResponse
    {
        public int Code { get; set; }
        public HecoChainPrices Prices { get; set; }
    }

    internal class HecoChainPrices
    {
        public int Fast { get; set; }
        public int Median { get; set; }
        public int Low { get; set; }
    }

    #endregion HecoChain Gas API Response

    #region Avalanche Gas API Response

    internal class AvalancheGasAPIResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public AvalancheResult Result { get; set; }
    }

    internal class AvalancheResult
    {
        public int Pendingcount { get; set; }
        public int Avgminingblocktxcountsize { get; set; }
        public int Avgtxnsperblock { get; set; }
        public double Mingaspricegwei { get; set; }
        public double Rapidgaspricegwei { get; set; }
        public double Fastgaspricegwei { get; set; }
        public double Standardgaspricegwei { get; set; }
    }

    #endregion Avalanche Gas API Response
}