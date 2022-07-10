using Nethereum.Web3;
using System.Numerics;
using xendfinance_dotnet_sdk.Functions.XAuto;
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
    public sealed class XAutoPolygonConnectorService : XAutoConnectorService, IXAutoPolygonConnectorService
    {
        private readonly IWeb3Client _web3Client;
        private string ERC20ContractABI = string.Empty;
        private string xAutoPolygonContractABI = string.Empty;

        public XAutoPolygonConnectorService(IWeb3Client web3Client) : base(web3Client)
        {
            _web3Client = web3Client;
        }

        public async Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, null, cancellationTokenSource, protocolContractAddress, amountInBase);

            if (!transactionResponse.IsSuccessful)
                throw new ContractTransactionException("Approval for xVault deposit failed");

            transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.Deposit, gasPriceLevel, null, cancellationTokenSource, amountInBase);
            if (!transactionResponse.IsSuccessful)
                throw new ContractTransactionException("xVault deposit failed");

            return transactionResponse;
        }

        public async Task<string> DepositAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, null, cancellationTokenSource, protocolContractAddress, amountInBase);

            if (!transactionResponse.IsSuccessful)
                throw new ContractTransactionException("Approval for xVault deposit failed");

            string transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.Deposit, gasPriceLevel, functionInput: amountInBase);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault deposit failed");
            return transactionHash;
        }

        public async Task<string> WithdrawBySharesAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset)
        {
            ReadContractABIs();

            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress);
            string transactionHash = await _web3Client.SendTransactionAsync(Networks.POLYGON, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.WithdrawShares, gasPriceLevel, null, sharesInBase);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

        public async Task<TransactionResponse> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();

            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.POLYGON, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, sharesInBase);
            return transactionResponse;
        }

        public async Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset);

            string transactionHash = await _web3Client.SendTransactionAsync(Networks.POLYGON, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.WithdrawShares, gasPriceLevel, null, shareEquivalent);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

        public async Task<TransactionResponse> WithdrawAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset);

            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.POLYGON, protocolContractAddress, xAutoPolygonContractABI, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, shareEquivalent);
            return transactionResponse;
        }

        public async Task<decimal> GetShareBalanceAsync(string address, Assets asset)
        {
            return await GetShareBalanceAsync(address, asset, Networks.POLYGON);
        }

        public async Task<decimal> GetPricePerFullShareAsync(Assets asset)
        {
            return await GetPricePerShareAsync(asset, Networks.POLYGON);
        }

        public async Task<decimal> CalculatePoolValueInToken(Assets asset)
        {
            return await CalculatePoolValueInToken(asset, Networks.POLYGON);
        }

        public async Task<string> FeeAddress(Assets asset)
        {
            return await FeeAddress(asset, Networks.POLYGON);
        }

        public async Task<decimal> FeeAmount(Assets asset)
        {
            return await FeeAmount(asset, Networks.POLYGON);
        }

        public async Task<LenderPolygon> Recommend(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            RecommendFunction function = new RecommendFunction();
            RecommendFunctionOutputDTO output = await _web3Client.CallContract<RecommendFunctionOutputDTO, RecommendFunction>(Networks.POLYGON, protocolContractAddress, function);
            BigInteger lender = output.Lender;
            return Enum.Parse<LenderPolygon>(lender.ToString());
        }

        public async Task<string> Token(Assets asset)
        {
            return await Token(asset, Networks.POLYGON);
        }

        public async Task<decimal> BalanceFulcrum(Assets asset)
        {
            return await BalanceFulcrum(asset, Networks.POLYGON);
        }

        public async Task<decimal> BalanceFulcrumInToken(Assets asset)
        {
            return await BalanceFulcrumInToken(asset, Networks.POLYGON);
        }

        public async Task<decimal> BalanceFortube(Assets asset)
        {
            return await BalanceFortube(asset, Networks.POLYGON);
        }

        public async Task<decimal> BalanceFortubeInToken(Assets asset)
        {
            return await BalanceFortubeInToken(asset, Networks.POLYGON);
        }

        public async Task<decimal> BalanceAave(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceAaveFunction function = new BalanceAaveFunction();
            BalanceAaveOutputDTO output = await _web3Client.CallContract<BalanceAaveOutputDTO, BalanceAaveFunction>(networks, protocolContractAddress, function);
            string fortubeTokenContractAddress = await AaveToken(asset, networks);
            return await ConvertBaseUnitToAmount(output.Balance, networks, fortubeTokenContractAddress);
        }

        public async Task<string> AaveToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            AaveTokenFunction function = new AaveTokenFunction();
            AaveTokenFunctionOutputDTO output = await _web3Client.CallContract<AaveTokenFunctionOutputDTO, AaveTokenFunction>(networks, protocolContractAddress, function);
            return output.Token;
        }

        public async Task<decimal> GetWorthOfSharesAsync(string address, Assets asset)
        {
            return await GetWorthOfSharesAsync(address, asset, Networks.POLYGON);
        }

        private async Task<BigInteger> ConvertAmountToBaseUnit(decimal amount, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(assetContractAddress, Networks.POLYGON); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
            int dp = int.Parse(decimalPlaces.ToString()); // converts string representation of BigInteger to int type
            BigInteger result = Web3.Convert.ToWei(amount, dp);
            return result;
        }

        private void ReadContractABIs()
        {
            ReadProtocolContractAbi();
            ReadERC20ContractAbi();
        }

        private void ReadProtocolContractAbi()
        {
            if (!string.IsNullOrWhiteSpace(xAutoPolygonContractABI))
            {
                return;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "XAuto", "Polygon", "xAuto.json");
            using (StreamReader r = new StreamReader(path))
            {
                string contractABI = r.ReadToEnd();
                xAutoPolygonContractABI = contractABI;
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
                case Assets.AAVE:
                    contractAddress = ProtocolContractAddresses.AAVE_AUTO_POLYGON_CONTRACT_ADDRESS;
                    break;

                case Assets.USDC:
                    contractAddress = ProtocolContractAddresses.USDC_AUTO_POLYGON_CONTRACT_ADDRESS;
                    break;

                case Assets.USDT:
                    contractAddress = ProtocolContractAddresses.USDT_AUTO_POLYGON_CONTRACT_ADDRESS;
                    break;

                case Assets.WBTC:
                    contractAddress = ProtocolContractAddresses.WBTC_AUTO_POLYGON_CONTRACT_ADDRESS;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Asset not supported on network for xAuto-Polygon");
            }
            return contractAddress;
        }

        protected override string GetAssetContractAddress(Assets asset)
        {
            string contractAddress;
            switch (asset)
            {
                case Assets.BUSD:
                    contractAddress = AssetContractAddresses.BUSD_POLYGON;
                    break;

                case Assets.USDC:
                    contractAddress = AssetContractAddresses.USDC_POLYGON;
                    break;

                case Assets.USDT:
                    contractAddress = AssetContractAddresses.USDT_POLYGON;
                    break;

                case Assets.BNB:
                    contractAddress = AssetContractAddresses.BNB_POLYGON;
                    break;

                case Assets.AAVE:
                    contractAddress = AssetContractAddresses.AAVE_POLYGON;
                    break;

                case Assets.WBTC:
                    contractAddress = AssetContractAddresses.WBTC_POLYGON;
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

        private async Task<BigInteger> GetPricePerFullShare(Assets asset)
        {
            return await GetPricePerFullShare(asset, Networks.POLYGON);
        }
    }
}