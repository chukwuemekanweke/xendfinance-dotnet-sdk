using System.Numerics;
using xendfinance_dotnet_sdk.Functions.XAuto;
using xendfinance_dotnet_sdk.Functions.XAuto.BSC;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.Exceptions;
using xendfinance_dotnet_sdk.Models.ServiceModels;
using xendfinance_dotnet_sdk.OutputDTOs.XAuto;
using xendfinance_dotnet_sdk.OutputDTOs.XAuto.BSC;
using xendfinance_dotnet_sdk.Services.XAuto.Interfaces;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services.XAuto
{
    public sealed class XAutoBSCConnectorService : XAutoConnectorService, IXAutoBSCConnectorService
    {
        private string XAutoBNBContractABI = string.Empty;
        private string XAutoERC20ContractABI = string.Empty;

        private string ERC20ContractABI = string.Empty;
        private readonly IWeb3Client _web3Client;

        public XAutoBSCConnectorService(IWeb3Client web3Client) : base(web3Client)
        {
            _web3Client = web3Client;
        }

        public async Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            string abi = RetrieveABIBasedOnAsset(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress, Networks.BSC);
            TransactionResponse transactionResponse = null;

            switch (asset)
            {
                case Assets.BUSD:
                case Assets.USDC:
                case Assets.USDT:
                    transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, null, cancellationTokenSource, protocolContractAddress, amountInBase);

                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("Approval for xVault deposit failed");

                    transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, abi, FunctionNames.Deposit, gasPriceLevel, null, cancellationTokenSource, amountInBase);
                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("xVault deposit failed");

                    return transactionResponse;

                case Assets.BNB:
                    transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, ERC20ContractABI, FunctionNames.Deposit, gasPriceLevel, amount, cancellationTokenSource, amountInBase);
                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("xVault deposit failed");
                    return transactionResponse;

                default:
                    throw new ArgumentOutOfRangeException("Asset not supported");
            }
        }

        public async Task<string> DepositAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            string abi = RetrieveABIBasedOnAsset(asset);

            string tokenContractAddress = GetAssetContractAddress(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress, Networks.BSC);
            string transactionHash = string.Empty;

            switch (asset)
            {
                case Assets.BUSD:
                case Assets.USDC:
                case Assets.USDT:
                    TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, null, cancellationTokenSource, protocolContractAddress, amountInBase);

                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("Approval for xVault deposit failed");

                    transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, abi, FunctionNames.Deposit, gasPriceLevel, functionInput: amountInBase);
                    if (string.IsNullOrWhiteSpace(transactionHash))
                        throw new ContractTransactionException("xVault deposit failed");
                    return transactionHash;

                case Assets.BNB:
                    transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, abi, FunctionNames.Deposit, gasPriceLevel, value: amount);
                    if (string.IsNullOrWhiteSpace(transactionHash))
                        throw new ContractTransactionException("xVault deposit failed");
                    return transactionHash;

                default:
                    throw new ArgumentOutOfRangeException("Asset not supported");
            }
        }

        public async Task<string> WithdrawBySharesAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset)
        {
            string abi = RetrieveABIBasedOnAsset(asset);

            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress, Networks.BSC);
            string transactionHash = await _web3Client.SendTransactionAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, sharesInBase);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

        public async Task<TransactionResponse> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource)
        {
            string abi = RetrieveABIBasedOnAsset(asset);

            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress, Networks.BSC);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, sharesInBase);
            return transactionResponse;
        }

        public async Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset)
        {
            string abi = RetrieveABIBasedOnAsset(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress, Networks.BSC);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset);

            string transactionHash = await _web3Client.SendTransactionAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, shareEquivalent);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

        public async Task<TransactionResponse> WithdrawAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource)
        {
            string abi = RetrieveABIBasedOnAsset(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress, Networks.BSC);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset);

            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, shareEquivalent);
            return transactionResponse;
        }

        public async Task<decimal> GetShareBalanceAsync(string address, Assets asset)
        {
            return await GetShareBalanceAsync(address, asset, Networks.BSC);
        }

        public async Task<decimal> GetPricePerFullShareAsync(Assets asset)
        {
            return await GetPricePerShareAsync(asset, Networks.BSC);
        }

        public async Task<decimal> CalculatePoolValueInToken(Assets asset)
        {
            return await CalculatePoolValueInToken(asset, Networks.BSC);
        }

        public async Task<string> FeeAddress(Assets asset)
        {
            return await FeeAddress(asset, Networks.BSC);
        }

        public async Task<decimal> FeeAmount(Assets asset)
        {
            return await FeeAmount(asset, Networks.BSC);
        }

        public async Task<LenderBSC> Recommend(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            RecommendFunction function = new RecommendFunction();
            RecommendFunctionOutputDTO output = await _web3Client.CallContract<RecommendFunctionOutputDTO, RecommendFunction>(Networks.BSC, protocolContractAddress, function);
            BigInteger lender = output.Lender;
            return Enum.Parse<LenderBSC>(lender.ToString());
        }

        public async Task<string> Token(Assets asset)
        {
            return await Token(asset, Networks.BSC);
        }

        public async Task<BigInteger> BalanceAlpaca(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceAlpacaFunction function = new BalanceAlpacaFunction();
            BalanceAlpacaOutputDTO output = await _web3Client.CallContract<BalanceAlpacaOutputDTO, BalanceAlpacaFunction>(Networks.BSC, protocolContractAddress, function);
            return output.Balance;
        }

        public async Task<BigInteger> BalanceAlpacaInToken(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceAlpacaInTokenFunction function = new BalanceAlpacaInTokenFunction();
            BalanceAlpacaInTokenOutputDTO output = await _web3Client.CallContract<BalanceAlpacaInTokenOutputDTO, BalanceAlpacaInTokenFunction>(Networks.BSC, protocolContractAddress, function);
            return output.Balance;
        }

        public async Task<decimal> BalanceFulcrum(Assets asset)
        {
            return await BalanceFulcrum(asset, Networks.BSC);
        }

        public async Task<decimal> BalanceFulcrumInToken(Assets asset)
        {
            return await BalanceFulcrumInToken(asset, Networks.BSC);
        }

        public async Task<decimal> BalanceFortube(Assets asset)
        {
            return await BalanceFortube(asset, Networks.BSC);
        }

        public async Task<decimal> BalanceFortubeInToken(Assets asset)
        {
            return await BalanceFortubeInToken(asset, Networks.BSC);
        }

        public async Task<decimal> BalanceVenus(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceVenusFunction function = new BalanceVenusFunction();
            BalanceAaveOutputDTO output = await _web3Client.CallContract<BalanceAaveOutputDTO, BalanceVenusFunction>(Networks.BSC, protocolContractAddress, function);
            string venusTokenContractAddress = await VenusToken(asset, Networks.BSC);
            return await ConvertBaseUnitToAmount(output.Balance, Networks.BSC, venusTokenContractAddress);
        }

        public async Task<decimal> BalanceVenusInToken(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceVenusInTokenFunction function = new BalanceVenusInTokenFunction();
            BalanceVenusInTokenOutputDTO output = await _web3Client.CallContract<BalanceVenusInTokenOutputDTO, BalanceVenusInTokenFunction>(Networks.BSC, protocolContractAddress, function);
            return await ConvertBaseUnitToAmount(output.Balance, Networks.BSC, protocolContractAddress);
        }

        public async Task<string> VenusToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            VenusTokenFunction function = new VenusTokenFunction();
            AaveTokenFunctionOutputDTO output = await _web3Client.CallContract<AaveTokenFunctionOutputDTO, VenusTokenFunction>(networks, protocolContractAddress, function);
            return output.Token;
        }

        private string RetrieveABIBasedOnAsset(Assets asset)
        {
            ReadContractABIs();
            string abi;
            switch (asset)
            {
                case Assets.BUSD:
                case Assets.USDC:
                case Assets.USDT:
                    abi = XAutoERC20ContractABI;
                    break;

                case Assets.BNB:
                    abi = XAutoBNBContractABI;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Asset not supported");
            }

            return abi;
        }

        private async Task<BigInteger> GetPricePerFullShare(Assets asset)
        {
            return await GetPricePerFullShare(asset, Networks.BSC);
        }

        public async Task<decimal> GetWorthOfSharesAsync(string address, Assets asset)
        {
            return await GetWorthOfSharesAsync(address, asset, Networks.BSC);
        }

        private void ReadContractABIs()
        {
            ReadProtocolContractAbi();
            ReadERC20ContractAbi();
        }

        private void ReadProtocolContractAbi()
        {
            if (!string.IsNullOrWhiteSpace(XAutoBNBContractABI) && !string.IsNullOrWhiteSpace(XAutoERC20ContractABI))
            {
                return;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "XAuto", "BSC", "xBNB.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                XAutoBNBContractABI = contractABI;
            }

            path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "XAuto", "BSC", "xErc20.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                XAutoERC20ContractABI = contractABI;
            }

            return;
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

        protected override string GetProtocolContractAddress(Assets asset)
        {
            string contractAddress;
            switch (asset)
            {
                case Assets.BUSD:
                    contractAddress = ProtocolContractAddresses.BUSD_AUTO_BSC_CONTRACT_ADDRESS;
                    break;

                case Assets.USDC:
                    contractAddress = ProtocolContractAddresses.USDC_AUTO_BSC_CONTRACT_ADDRESS;
                    break;

                case Assets.USDT:
                    contractAddress = ProtocolContractAddresses.USDT_AUTO_BSC_CONTRACT_ADDRESS;
                    break;

                case Assets.BNB:
                    contractAddress = ProtocolContractAddresses.BNB_AUTO_BSC_CONTRACT_ADDRESS;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Asset not supported on network for xAuto-BSC");
            }
            return contractAddress;
        }

        protected override string GetAssetContractAddress(Assets asset)
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

        private async Task<BigInteger> GetShareEquivalentOfAmount(BigInteger amount, Assets asset)
        {
            BigInteger pricePerShare = await GetPricePerFullShare(asset);
            BigInteger share = BigInteger.Divide(amount, pricePerShare);
            return share;
        }
    }
}