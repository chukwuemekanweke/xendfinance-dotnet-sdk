using Microsoft.Extensions.DependencyInjection;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Services;
using xendfinance_dotnet_sdk.Services.XAuto;
using xendfinance_dotnet_sdk.Services.XAuto.Interfaces;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Installers
{
    public static class SdkInstaller
    {
        public static void AddXendFinanceSdk(this IServiceCollection services,
                                             string privateKey,
                                             GasPriceLevel gasPriceLevel = GasPriceLevel.Average)
        {
            RegisterServices(services, GasEstimateUrls.BSCGasEstimateUrl, GasEstimateUrls.PolygonGasEstimateUrl);
            services.AddSingleton<IWeb3Client>(x =>
            {
                IGasEstimatorService gasEstimatorService = x.GetRequiredService<IGasEstimatorService>();
                return new Web3Client(privateKey, ChainIds.BSCMainnet, ChainIds.PolygonMainnet, RPCNodeUrls.BSC_MAINNET, RPCNodeUrls.POLYGON_MAINNET, gasPriceLevel, gasEstimatorService);
            });
        }

        public static void AddXendFinanceSdk(this IServiceCollection services,
                                            string privateKey,
                                            string? bscNodeUrl = null,
                                            string? polygonNodeUrl = null,
                                            string? bscGasEstimateUrl = null,
                                            string? polygonGasEstimateUrl = null,
                                            GasPriceLevel gasPriceLevel = GasPriceLevel.Average,
                                            BlockchainEnvironment environment = BlockchainEnvironment.Mainnet)
        {
            if (string.IsNullOrWhiteSpace(bscGasEstimateUrl))
            {
                bscGasEstimateUrl = GasEstimateUrls.BSCGasEstimateUrl;
            }

            if (string.IsNullOrWhiteSpace(polygonGasEstimateUrl))
            {
                polygonGasEstimateUrl = GasEstimateUrls.PolygonGasEstimateUrl;
            }

            RegisterServices(services, bscGasEstimateUrl, polygonGasEstimateUrl);
            switch (environment)
            {
                case BlockchainEnvironment.Mainnet:
                    services.AddSingleton<IWeb3Client>(x =>
                    {
                        IGasEstimatorService gasEstimatorService = x.GetRequiredService<IGasEstimatorService>();
                        return new Web3Client(privateKey, ChainIds.BSCMainnet, ChainIds.PolygonMainnet, bscNodeUrl ?? RPCNodeUrls.BSC_MAINNET, polygonNodeUrl ?? RPCNodeUrls.POLYGON_MAINNET, gasPriceLevel, gasEstimatorService);
                    });
                    break;

                case BlockchainEnvironment.Testnet:
                    services.AddSingleton<IWeb3Client>(x =>
                    {
                        IGasEstimatorService gasEstimatorService = x.GetRequiredService<IGasEstimatorService>();
                        return new Web3Client(privateKey, ChainIds.BSCTestnet, ChainIds.PolygonTestnet, bscNodeUrl ?? RPCNodeUrls.BSC_TESTNET, polygonNodeUrl ?? RPCNodeUrls.POLYGON_TESTNET, gasPriceLevel, gasEstimatorService);
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Found unsupported blochchain environment");
            }
        }

        private static void RegisterServices(IServiceCollection services, string bscGasEstimateUrl, string polygonGasEstimateUrl)
        {
            services.AddSingleton<IGasEstimatorService>(x =>
            {
                IHttpClientFactory httpClientFactory = x.GetRequiredService<IHttpClientFactory>();
                return new GasEstimatorService(bscGasEstimateUrl, polygonGasEstimateUrl, httpClientFactory);
            });
            services.AddTransient<IXVaultConnectorService, XVaultConnectorService>();
            services.AddTransient<IXAutoBSCConnectorService, XAutoBSCConnectorService>();
            services.AddTransient<IXAutoPolygonConnectorService, XAutoPolygonConnectorService>();
        }
    }
}