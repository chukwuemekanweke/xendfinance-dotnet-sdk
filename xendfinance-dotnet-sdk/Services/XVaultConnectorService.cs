using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services
{
    internal class XVaultConnectorService : IXVaultConnectorService
    {

        private string XVaultContractABI;
        private string ERC20ContractABI;
        private readonly IWeb3Client _web3Client;

        public XVaultConnectorService(IWeb3Client web3Client)
        {
            _web3Client = web3Client;
        }

        public Task<string> DepositAndWaitForReceiptAsync(decimal amount, Assets asset, Networks network = Networks.BSC, GasPriceLevel? gasPriceLevel, CancellationToken cancellationToken)
        {
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            BigInteger amountInBase = ConvertAmountToBaseUnit(amount);
            TransactionResponse transactionResponse = _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, cancellationToken, protocolContractAddress, amountInBase);

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

        private BigInteger ConvertAmountToBaseUnit(decimal amount)
        {
            throw new NotImplementedException();

        }

        private void ReadContractABIs()
        {
            ReadProtocolContractAbi();
            ReadERC20ContractAbi();
        }

        private string ReadProtocolContractAbi()
        {
            if(!string.IsNullOrWhiteSpace(XVaultContractABI))
            {
                return XVaultContractABI;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "xVault.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                XVaultContractABI = contractABI;
                return contractABI;
            }
        }

        private string ReadERC20ContractAbi()
        {
            if (!string.IsNullOrWhiteSpace(ERC20ContractABI))
            {
                return ERC20ContractABI;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "erc20.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                ERC20ContractABI = contractABI;
                return contractABI;
            }
        }

        private string GetProtocolContractAddress(Assets asset, Networks network)
        {
            string contractAddress;
            switch (network)
            {
                case Networks.BSC:
                    switch (asset)
                    {
                        case Assets.BUSD:
                            contractAddress = ProtocolContractAddresses.BUSD_VAULT_BSC_CONTRACT_ADDRESS;
                            break;
                        case Assets.USDC:
                            contractAddress = ProtocolContractAddresses.USDC_VAULT_BSC_CONTRACT_ADDRESS;
                            break;
                        case Assets.USDT:
                            contractAddress = ProtocolContractAddresses.USDT_VAULT_BSC_CONTRACT_ADDRESS;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Asset not supported on network for xVault");
                    }
                    break;
                case Networks.POLYGON:
                default:
                    throw new ArgumentOutOfRangeException("Network not supported for xVault");

            }
            return contractAddress;
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
