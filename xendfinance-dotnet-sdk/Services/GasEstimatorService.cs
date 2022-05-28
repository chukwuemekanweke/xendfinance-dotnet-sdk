using Newtonsoft.Json;
using System.Collections.Concurrent;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.ServiceModels;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services
{
    internal class GasEstimatorService : IGasEstimatorService
    {
        private readonly HttpClient _bscHttpClient;
        private readonly HttpClient _polygonHttpClient;
        private readonly string _bscGasEstimateUrl;
        private readonly string _polygonGasEstimateUrl;

        private ConcurrentDictionary<Networks, GasEstimateResponse> gasEstimates = new ConcurrentDictionary<Networks, GasEstimateResponse>();
        private const int TTL_IN_MINUTES = 5;

        public GasEstimatorService(string bscGasEstimateUrl, string polygonGasEstimateUrl, IHttpClientFactory httpClientFactory)
        {
            _bscGasEstimateUrl = bscGasEstimateUrl;
            _polygonGasEstimateUrl = polygonGasEstimateUrl;
            _bscHttpClient = httpClientFactory.CreateClient("xendfinance-dotnet-sdk-gas-client-bsc");
            _polygonHttpClient = httpClientFactory.CreateClient("xendfinance-dotnet-sdk-gas-client-polygon");

        }



        /// <summary>
        /// Get's the estimated gas cost for a blockchain network
        /// The gas estimate gotten is cached and API calls to the Gas estimate API's ar eonly made once an entry is not found in the cache
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<GasEstimateResponse> EstimateGas(Networks network)
        {

            GasEstimateResponse gasEstimate = GetFromInMemory(network);
            if (gasEstimate != null)
            {
                return gasEstimate;
            }

            switch (network)
            {
                case Networks.BSC:
                    gasEstimate = await EstimateGasBSC();
                    break;
                case Networks.POLYGON:
                    gasEstimate = await EstimateGasPolygon();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported Network Chain");
            }
            AddToInMemory(network, gasEstimate);
            return gasEstimate;
        }

        private async Task<GasEstimateResponse> EstimateGasBSC()
        {
            HttpResponseMessage httpResponseMessage = await _bscHttpClient.GetAsync(_bscGasEstimateUrl);
            httpResponseMessage.EnsureSuccessStatusCode();
            string contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            BSCGasEstimateResponse response = JsonConvert.DeserializeObject<BSCGasEstimateResponse>(contentString);
            return new GasEstimateResponse
            {
                AverageGas = response.Result.ProposeGasPrice,
                FastGas = response.Result.FastGasPrice,
                LowGas = response.Result.SafeGasPrice
            };
        }

        private async Task<GasEstimateResponse> EstimateGasPolygon()
        {
            HttpResponseMessage httpResponseMessage = await _polygonHttpClient.GetAsync(_polygonGasEstimateUrl);
            httpResponseMessage.EnsureSuccessStatusCode();
            string contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            PolygonGasEstimateResponse response = JsonConvert.DeserializeObject<PolygonGasEstimateResponse>(contentString);
            return new GasEstimateResponse
            {
                AverageGas = response.Result.ProposeGasPrice,
                FastGas = response.Result.FastGasPrice,
                LowGas = response.Result.SafeGasPrice
            };
        }

        private void AddToInMemory(Networks network, GasEstimateResponse response)
        {
            response.UpdatedDate = DateTime.UtcNow;
            gasEstimates[network] = response;
        }

        private GasEstimateResponse? GetFromInMemory(Networks network)
        {
            if (!gasEstimates.ContainsKey(network))
            {
                return null;
            }

            double minutesElapsed = (DateTime.UtcNow - gasEstimates[network].UpdatedDate).TotalMinutes;
            if (minutesElapsed > TTL_IN_MINUTES)
            {
                gasEstimates[network] = null;
            }

            return gasEstimates[network];
        }
    }
}
