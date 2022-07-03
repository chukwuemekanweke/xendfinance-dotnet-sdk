using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.Web3;
using System.Numerics;
using System.Text;
using xendfinance_dotnet_sdk.Functions.XAuto;
using xendfinance_dotnet_sdk.Functions.XVault;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Models.Exceptions;
using xendfinance_dotnet_sdk.Models.ServiceModels;
using xendfinance_dotnet_sdk.OutputDTOs.XAuto;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services
{
    internal class XAutoBSCConnectorService: ERC20Primitive
    {

        private string XAutoBNBContractABI = string.Empty;
        private string XAutoERC20ContractABI = string.Empty;

        private string ERC20ContractABI = string.Empty;
        private readonly IWeb3Client _web3Client;

        public XAutoBSCConnectorService(IWeb3Client web3Client): base(web3Client)
        {
            _web3Client = web3Client;
        }

        public async Task<TransactionResponse> DepositAndWaitForReceiptAsync(decimal amount, GasPriceLevel? gasPriceLevel, Assets asset, Networks network, CancellationTokenSource? cancellationTokenSource)
        {
            string abi = RetrieveABIBasedOnAsset(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
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
            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
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
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress);
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
            BigInteger sharesInBase = await ConvertAmountToBaseUnit(shares, tokenContractAddress);
            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, sharesInBase);
            return transactionResponse;
        }

        public async Task<string> WithdrawAsync(decimal amount, string recipientAddress, double maxLossPercentage, GasPriceLevel? gasPriceLevel, Assets asset)
        {
            string abi = RetrieveABIBasedOnAsset(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
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

            BigInteger amountInBase = await ConvertAmountToBaseUnit(amount, tokenContractAddress);
            BigInteger shareEquivalent = await GetShareEquivalentOfAmount(amountInBase, asset);

            TransactionResponse transactionResponse = await _web3Client.SendTransactionAndWaitForReceiptAsync(Networks.BSC, protocolContractAddress, abi, FunctionNames.WithdrawShares, gasPriceLevel, null, cancellationTokenSource, shareEquivalent);
            return transactionResponse;
        }

        public async Task<decimal> GetShareBalanceAsync(string address, Assets asset)
        {
            _ = RetrieveABIBasedOnAsset(asset);
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger balanceInBaseUnit = await GetBalanceOf(protocolContractAddress, address, Networks.BSC);
            decimal balance = await ConvertBaseUnitToAmount(balanceInBaseUnit, protocolContractAddress);
            return balance;
        }

        public async Task<decimal> GetPricePerFullShareAsync(Assets asset)
        {
            _ = RetrieveABIBasedOnAsset(asset);
            string tokenContractAddress = GetAssetContractAddress(asset);
            BigInteger pricePerShareBaseUnit = await GetPricePerFullShare(asset);
            decimal pricePerShare = await ConvertBaseUnitToAmount(pricePerShareBaseUnit, tokenContractAddress);
            return pricePerShare;
        }

        public async Task<BigInteger> CalculatePoolValueInToken(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            CalculatePoolValueInTokenFunction function = new CalculatePoolValueInTokenFunction();
            CalculatePoolValueInTokenOutputDTO output = await _web3Client.CallContract<CalculatePoolValueInTokenOutputDTO, CalculatePoolValueInTokenFunction>(Networks.BSC, protocolContractAddress, function);
            BigInteger value = output.Value;
            return value;
        }

        public async Task<string> FeeAddress(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FeeAddressFunction function = new FeeAddressFunction();
            FeeAddressOutputDTO output = await _web3Client.CallContract<FeeAddressOutputDTO, FeeAddressFunction>(Networks.BSC, protocolContractAddress, function);
            return output.Address;
        }

        public async Task<BigInteger> FeeAmount(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FeeAmountFunction function = new FeeAmountFunction();
            FeeAmountDTO output = await _web3Client.CallContract<FeeAmountDTO, FeeAmountFunction>(Networks.BSC, protocolContractAddress, function);
            return output.Amount;
        }

        public async Task<string> FeeAddress(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            RecommendFunction function = new RecommendFunction();
            RecommendFunctionOutputDTO output = await _web3Client.CallContract<RecommendFunctionOutputDTO, FeeAddressFunction>(Networks.BSC, protocolContractAddress, function);
            BigInteger lender = output.Lender;
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

        private async Task<decimal> ConvertBaseUnitToAmount(BigInteger amount, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(assetContractAddress); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
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

        private async Task<BigInteger> ConvertAmountToBaseUnit(decimal amount, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(assetContractAddress); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
            int dp = int.Parse(decimalPlaces.ToString()); // converts string representation of BigInteger to int type
            BigInteger result = Web3.Convert.ToWei(amount, dp);
            return result;
        }

        private async Task<BigInteger> GetDecimals(string assetContractAddress)
        {
            DecimalsFunction decimalsFunction = new DecimalsFunction();
            DecimalsOutputDTO output = await _web3Client.CallContract<DecimalsOutputDTO, DecimalsFunction>(Networks.BSC, assetContractAddress, decimalsFunction);
            return output.Decimals;
        }




        private async Task<BigInteger> GetPricePerFullShare(Assets asset)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            GetPricePerFullShareFunction pricePerFullShareFunction = new GetPricePerFullShareFunction();
            GetPricePerFullShareOutputDTO output = await _web3Client.CallContract<GetPricePerFullShareOutputDTO, GetPricePerFullShareFunction>(Networks.BSC, protocolContractAddress, pricePerFullShareFunction);
            return output.Value;
        }

        public async Task<decimal> GetWorthOfSharesAsync(string address, Assets asset)
        {
            ReadContractABIs();
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger balanceInBaseUnit = await GetShareBalance(protocolContractAddress, address, asset);
            BigInteger pricePerShareInBaseUnit = await GetPricePerFullShare(asset);
            BigInteger shareValueInBaseUnit = BigInteger.Multiply(pricePerShareInBaseUnit, balanceInBaseUnit);
            decimal shareValue = await ConvertBaseUnitToAmount(shareValueInBaseUnit, protocolContractAddress);
            return shareValue;
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

        private string GetProtocolContractAddress(Assets asset)
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
                    throw new ArgumentOutOfRangeException("Asset not supported on network for xVault");
            }
            return contractAddress;
        }


        private string GetAssetContractAddress(Assets asset)
        {
            string contractAddress = GetBSCAssetContractAddress(asset);           
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

        private async Task<BigInteger> GetShareEquivalentOfAmount(BigInteger amount, Assets asset)
        {
            BigInteger pricePerShare = await GetPricePerFullShare(asset);
            BigInteger share = BigInteger.Divide(amount, pricePerShare);
            return share;
        }
    }
}
