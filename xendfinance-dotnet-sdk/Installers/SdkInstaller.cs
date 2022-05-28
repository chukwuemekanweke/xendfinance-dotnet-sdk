using Microsoft.Extensions.DependencyInjection;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Services;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Installers
{
    public static class SdkInstaller
    {
        public static void AddXendFinanceSdk(this IServiceCollection services, string privateKey)
        {
            RegisterServices(services);
            services.AddSingleton<IWeb3Client>(x => new Web3Client(privateKey, ChainIds.BSCMainnet, ChainIds.PolygonMainnet, RPCNodeUrls.BSC_MAINNET, RPCNodeUrls.POLYGON_MAINNET));
        }

        public static void AddXendFinanceSdk(this IServiceCollection services, string privateKey, BlockchainEnvironment environment = BlockchainEnvironment.Mainnet)
        {
            RegisterServices(services);
            switch (environment)
            {
                case BlockchainEnvironment.Mainnet:
                    services.AddSingleton<IWeb3Client>(x => new Web3Client(privateKey, ChainIds.BSCMainnet, ChainIds.PolygonMainnet, RPCNodeUrls.BSC_MAINNET, RPCNodeUrls.POLYGON_MAINNET));
                    break;
                case BlockchainEnvironment.Testnet:
                    services.AddSingleton<IWeb3Client>(x => new Web3Client(privateKey, ChainIds.BSCTestnet, ChainIds.PolygonTestnet, RPCNodeUrls.BSC_TESTNET, RPCNodeUrls.POLYGON_TESTNET));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Found unsupported blochchain environment");
            }
        }

        public static void AddXendFinanceSdk(this IServiceCollection services, string privateKey, string bscNodeUrl, string polygonNodeUrl, BlockchainEnvironment environment = BlockchainEnvironment.Mainnet)
        {
            RegisterServices(services);
            switch (environment)
            {
                case BlockchainEnvironment.Mainnet:
                    services.AddSingleton<IWeb3Client>(x => new Web3Client(privateKey, ChainIds.BSCMainnet, ChainIds.PolygonMainnet, bscNodeUrl, polygonNodeUrl));
                    break;
                case BlockchainEnvironment.Testnet:
                    services.AddSingleton<IWeb3Client>(x => new Web3Client(privateKey, ChainIds.BSCTestnet, ChainIds.PolygonTestnet, bscNodeUrl, polygonNodeUrl));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Found unsupported blochchain environment");
            }
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IXVaultConnectorService, XVaultConnectorService>();
        }

    }
}
