using System.Numerics;
using System.Text;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.Exceptions;
using xendfinance_dotnet_sdk.Models.ServiceModels;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services
{
    internal class XAutoBSCConnectorService
    {

        private string XAutoBNBContractABI = string.Empty;
        private string XAutoERC20ContractABI = string.Empty;

        private string ERC20ContractABI = string.Empty;
        private readonly IWeb3Client _web3Client;

        public XAutoBSCConnectorService(IWeb3Client web3Client)
        {
            _web3Client = web3Client;
        }

        public async Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, network, tokenContractAddress);
            TransactionResponse transactionResponse = null;

            switch (asset)
            {
                case Assets.BUSD:
                case Assets.USDC:
                case Assets.USDT:
                    transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, null, cancellationTokenSource, protocolContractAddress, amountInBase);

                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("Approval for xVault deposit failed");

                    transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, xv, FunctionNames.Deposit, gasPriceLevel, null, cancellationTokenSource, amountInBase);
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
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, network, tokenContractAddress);
            string transactionHash = string.Empty;

            switch (asset)
            {
                case Assets.BUSD:
                case Assets.USDC:
                case Assets.USDT:
                    TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, tokenContractAddress, ERC20ContractABI, FunctionNames.Approve, gasPriceLevel, cancellationTokenSource, protocolContractAddress, amountInBase);

                    if (!transactionResponse.IsSuccessful)
                        throw new ContractTransactionException("Approval for xVault deposit failed");

                    transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, ERC20ContractABI, FunctionNames.Deposit, gasPriceLevel, functionInput: amountInBase);
                    if (string.IsNullOrWhiteSpace(transactionHash))
                        throw new ContractTransactionException("xVault deposit failed");
                    return transactionHash;
                case Assets.BNB:
                    transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, ERC20ContractABI, FunctionNames.Deposit, gasPriceLevel, value: amount);
                    if (string.IsNullOrWhiteSpace(transactionHash))
                        throw new ContractTransactionException("xVault deposit failed");
                    return transactionHash;
                default:
                    throw new ArgumentOutOfRangeException("Asset not supported");
            }
        }

        public async Task<string> WithdrawBySharesAsync(decimal shares, string recipientAddress, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC)
        {
            ReadContractABIs();
            string abi = RetrieveABIBasedOnAsset(asset);

            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, network, tokenContractAddress);
            string transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, sharesInBase);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

       

        public async Task<TransactionResponse> WithdrawBySharesAndWaitForReceiptAsync(decimal shares, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            ReadContractABIs();
            string abi = RetrieveABIBasedOnAsset(asset);

            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, network, tokenContractAddress);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, sharesInBase);
            return transactionResponse;
        }

        public async Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC)
        {
            ReadContractABIs();
            BigInteger maxLoss = ConvertMaxLossPercentage(maxLossPercentage);
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            string tokenContractAddress = GetAssetContractAddress(asset, network);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, network, tokenContractAddress);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset, network);

            string transactionHash = await _web3Client.SendTransactionAsync(network, protocolContractAddress, ERC20ContractABI, FunctionNames.WithdrawShares, gasPriceLevel, shareEquivalent, recipientAddress, maxLoss);
            if (string.IsNullOrWhiteSpace(transactionHash))
                throw new ContractTransactionException("xVault withdraw by shares failed");
            return transactionHash;
        }

        public async Task<TransactionResponse> WithdrawAndWaitForReceiptAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset, Networks network = Networks.BSC, CancellationToken cancellationToken = default)
        {
            ReadContractABIs();
            BigInteger maxLoss = ConvertMaxLossPercentage(maxLossPercentage);
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            string tokenContractAddress = GetAssetContractAddress(asset, network);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, network, tokenContractAddress);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset, network);

            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(network, protocolContractAddress, ERC20ContractABI, FunctionNames.WithdrawShares, gasPriceLevel, cancellationToken, shareEquivalent, recipientAddress, maxLoss);
            return transactionResponse;
        }

        public async Task<decimal> GetShareBalanceAsync(string address, Assets asset, Networks network)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            BigInteger balanceInBaseUnit = await GetShareBalance(protocolContractAddress, address, asset, network);
            decimal balance = await ConvertBaseUnitToAmount(balanceInBaseUnit, network, protocolContractAddress);
            return balance;
        }

       

        public async Task<decimal> GetPricePerShareAsync(Assets asset, Networks network)
        {
            ReadContractABIs();
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            BigInteger pricePerShareBaseUnit = await GetPricePerShare(asset, network);
            decimal pricePerShare = await ConvertBaseUnitToAmount(pricePerShareBaseUnit, network, tokenContractAddress);
            return pricePerShare;
        }

       

        public async Task<double> GetAPRAsync(Assets asset, Networks network)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            GetAPYFunction function = new GetAPYFunction();
            GetAPYOutputDTO output = await _web3Client.CallContract<GetAPYOutputDTO, GetAPYFunction>(network, protocolContractAddress, function);
            BigInteger apyInBaseUnit = output.APY;
            double apy = (double)(await ConvertBaseUnitToAmount(apyInBaseUnit, network, protocolContractAddress));
            return apy;
        }

        private string RetrieveABIBasedOnAsset(Assets asset)
        {
            string abi = string.Empty;

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

        private async Task<decimal> ConvertBaseUnitToAmount(BigInteger amount, Networks network, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(network, assetContractAddress); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
            decimal dp = decimal.Parse(decimalPlaces.ToString()); // converts string representation of BigInteger to int type
            decimal divisorValue = (decimal)Math.Pow(10, (int)dp); // 10 ^ dp
            BigInteger divisor = BigInteger.Parse(divisorValue.ToString()); // Get's the BigInteger representation of 10 ^ dp


            BigInteger quotient = BigInteger.Divide(amount, divisor);
            BigInteger remainder = BigInteger.Remainder(amount, divisor);
            decimal result;

            // if this code block gets confusing
            // See this quora feed
            // https://www.quora.com/How-do-you-convert-remainders-to-decimals-in-basic-math

            if (!remainder.Equals(0))
            {
                decimal remainderValue = decimal.Parse(remainder.ToString());
                decimal fractionalPart = remainderValue / divisorValue;
                decimal quotientValue = int.Parse(quotient.ToString());
                result = quotientValue + fractionalPart;
            }
            else
            {
                decimal quotientValue = int.Parse(quotient.ToString());
                result = quotientValue;
            }
            return result;
        }

        private async Task<BigInteger> ConvertAmountToBaseUnit(decimal amount, Networks network, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(network, assetContractAddress); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
            int dp = int.Parse(decimalPlaces.ToString()); // converts string representation of BigInteger to int type
            BigInteger result = Web3.Convert.ToWei(amount, dp);
            return result;
        }

        private async Task<BigInteger> GetDecimals(Networks network, string assetContractAddress)
        {
            DecimalsFunction decimalsFunction = new DecimalsFunction();
            DecimalsOutputDTO output = await _web3Client.CallContract<DecimalsOutputDTO, DecimalsFunction>(network, assetContractAddress, decimalsFunction);
            return output.Decimals;
        }

        private async Task<BigInteger> GetShareBalance(string contractAddress, string address, Assets asset, Networks network)
        {
            BalanceOfFunction function = new BalanceOfFunction()
            {
                Address = address
            };
            BalanceOfOutputDTO output = await _web3Client.CallContract<BalanceOfOutputDTO, BalanceOfFunction>(network, contractAddress, function);
            BigInteger balanceInBaseUnit = output.Balance;
            return balanceInBaseUnit;
        }

        private async Task<BigInteger> GetPricePerShare(Assets asset, Networks network)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            PricePerShareFunction pricePerShareFunction = new PricePerShareFunction();
            PricePerShareOutputDTO output = await _web3Client.CallContract<PricePerShareOutputDTO, PricePerShareFunction>(network, protocolContractAddress, pricePerShareFunction);
            return output.PricePerShare;
        }

        public async Task<decimal> GetWorthOfSharesAsync(string address, Assets asset, Networks network = Networks.BSC)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            BigInteger balanceInBaseUnit = await GetShareBalance(protocolContractAddress, address, asset, network);
            BigInteger pricePerShareInBaseUnit = await GetPricePerShare(asset, network);
            BigInteger shareValueInBaseUnit = BigInteger.Multiply(pricePerShareInBaseUnit, balanceInBaseUnit);
            decimal shareValue = await ConvertBaseUnitToAmount(shareValueInBaseUnit, network, protocolContractAddress);
            return shareValue;
        }

        public async Task<decimal> GetTotalAssetsAsync(Assets asset, Networks network = Networks.BSC)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset, network);
            TotalAssetsFunction function = new TotalAssetsFunction();
            TotalAssetsOutputDTO output = await _web3Client.CallContract<TotalAssetsOutputDTO, TotalAssetsFunction>(network, protocolContractAddress, function);
            BigInteger totalAssetsInBaseUnit = output.TotalAssets;
            decimal totalAssets = await ConvertBaseUnitToAmount(totalAssetsInBaseUnit, network, protocolContractAddress);
            return totalAssets;
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
                return ;
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), "ABIs", "XAuto", "BSC",  "xBNB.json");
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

        private string GetProtocolContractAddress(Assets asset, Networks network)
        {
            string contractAddress;
            switch (network)
            {
                case Networks.BSC:
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


        private BigInteger ConvertMaxLossPercentage(double maxLossPercentage)
        {
            int maxLoss = (int)Math.Round(maxLossPercentage / (double)100);
            return BigInteger.Parse(maxLoss.ToString());
        }

        private async Task<BigInteger> GetShareEquivalentOfAmount(BigInteger amount, Assets asset, Networks network)
        {
            BigInteger pricePerShare = await GetPricePerShare(asset, network);
            BigInteger share = BigInteger.Divide(amount, pricePerShare);
            return share;
        }
    }
}
