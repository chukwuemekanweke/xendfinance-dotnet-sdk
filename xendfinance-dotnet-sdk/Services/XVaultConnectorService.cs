using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services
{
    internal class XVaultConnectorService : IXVaultConnectorService
    {

        private string ContractABI;

        public Task<string> DepositAndWaitForReceiptAsync(decimal amount, Assets asset, Networks network = Networks.BSC)
        {
            string tokenContractAddress = GetAssetContractAddress(asset, network);

            throw new NotImplementedException();
        }


        public Task<string> DepositAsync(decimal amount, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<double> GetAPYAsync(Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetCreditAvailableAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetDebtOutstandingAsync(string strategyAddress, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetDepositLimit(Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetPricePerShareAsync(Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> MaxAvailableSharesAsync(Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> ShareBalanceAsync(string address, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawAndWaitForReceiptAsync(decimal amount, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawAsync(decimal amount, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<string> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        public Task<string> WithdrawBySharesAsync(decimal shares, Assets asset, Networks network = Networks.BSC)
        {
            throw new NotImplementedException();
        }

        private string ReadContractAbi()
        {
            if(!string.IsNullOrWhiteSpace(ContractABI))
            {
                return ContractABI;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "xVault.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                ContractABI = contractABI;
                return contractABI;
            }
        }


        private string GetAssetContractAddress(Assets asset, Networks network)
        {
            string contractAddress;
            switch (network)
            {
                case Networks.BSC:
                    contractAddress = GetBSCAssetContractAddress(asset);
                    break;
                case Networks.POLYGON:
                    contractAddress = GetPolygonAssetContractAddress(asset);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Network not supported for Xend Finance Protocol");

            }
            return contractAddress;
        }

        private string GetPolygonAssetContractAddress(Assets asset)
        {
            string contractAddress;
            switch (asset)
            {
                case Assets.BUSD:
                    contractAddress = AssetContractAddresses.BUSD_BSC;
                    break;
                case Assets.USDC:
                    contractAddress = AssetContractAddresses.USDC_BSC;
                    break;
                case Assets.USDT:
                    contractAddress = AssetContractAddresses.USDT_BSC;
                    break;
                case Assets.BNB:
                    contractAddress = AssetContractAddresses.BNB_BSC;
                    break;
                case Assets.AAVE:
                    contractAddress = AssetContractAddresses.AAVE_BSC;
                    break;
                case Assets.WBTC:
                    contractAddress = AssetContractAddresses.WBTC_BSC;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Asset is not supported on network");
            }

            return contractAddress;
        }

        private string GetBSCAssetContractAddress(Assets asset)
        {
            string contractAddress;
            switch (asset)
            {
                case Assets.BUSD:
                    contractAddress = AssetContractAddresses.BUSD_BSC;
                    break;
                case Assets.USDC:
                    contractAddress = AssetContractAddresses.USDC_BSC;
                    break;
                case Assets.USDT:
                    contractAddress = AssetContractAddresses.USDT_BSC;
                    break;
                case Assets.BNB:
                    contractAddress = AssetContractAddresses.BNB_BSC;
                    break;
                case Assets.AAVE:
                    contractAddress = AssetContractAddresses.AAVE_BSC;
                    break;
                case Assets.WBTC:
                    contractAddress = AssetContractAddresses.WBTC_BSC;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Asset is not supported on network");
            }

            return contractAddress;
        }
    }
}
