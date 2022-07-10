using Nethereum.Web3;
using System.Numerics;
using xendfinance_dotnet_sdk.Functions.XAuto;
using xendfinance_dotnet_sdk.Functions.XVault;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.OutputDTOs.XAuto;
using xendfinance_dotnet_sdk.OutputDTOs.XVault;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk.Services.XAuto
{
    public abstract class XAutoConnectorService : ERC20Primitive

    {
        private readonly IWeb3Client _web3Client;

        public XAutoConnectorService(IWeb3Client web3Client) : base(web3Client)
        {
            _web3Client = web3Client;
        }

        protected async Task<decimal> GetShareBalanceAsync(string address, Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger balanceInBaseUnit = await GetBalanceOf(protocolContractAddress, address, networks);
            decimal balance = await ConvertBaseUnitToAmount(balanceInBaseUnit, networks, protocolContractAddress);
            return balance;
        }

        protected async Task<decimal> GetPricePerFullShareAsync(Assets asset, Networks networks)
        {
            string tokenContractAddress = GetAssetContractAddress(asset, networks);
            BigInteger pricePerShareBaseUnit = await GetPricePerShare(asset, networks);
            decimal pricePerShare = await ConvertBaseUnitToAmount(pricePerShareBaseUnit, networks, tokenContractAddress);
            return pricePerShare;
        }

        protected async Task<decimal> CalculatePoolValueInToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            CalculatePoolValueInTokenFunction function = new CalculatePoolValueInTokenFunction();
            CalculatePoolValueInTokenOutputDTO output = await _web3Client.CallContract<CalculatePoolValueInTokenOutputDTO, CalculatePoolValueInTokenFunction>(networks, protocolContractAddress, function);
            BigInteger value = output.Value;
            return await ConvertBaseUnitToAmount(value, networks, protocolContractAddress);
        }

        protected async Task<string> FeeAddress(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FeeAddressFunction function = new FeeAddressFunction();
            FeeAddressOutputDTO output = await _web3Client.CallContract<FeeAddressOutputDTO, FeeAddressFunction>(networks, protocolContractAddress, function);
            return output.Address;
        }

        protected async Task<decimal> FeeAmount(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FeeAmountFunction function = new FeeAmountFunction();
            FeeAmountDTO output = await _web3Client.CallContract<FeeAmountDTO, FeeAmountFunction>(networks, protocolContractAddress, function);
            return await ConvertBaseUnitToAmount(output.Amount, networks, protocolContractAddress);
        }

        protected async Task<decimal> BalanceFulcrum(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceFulcrumFunction function = new BalanceFulcrumFunction();
            BalanceFulcrumOutputDTO output = await _web3Client.CallContract<BalanceFulcrumOutputDTO, BalanceFulcrumFunction>(networks, protocolContractAddress, function);
            string fulcrumTokenContractAddress = await FulcrumToken(asset, networks);
            return await ConvertBaseUnitToAmount(output.Balance, networks, fulcrumTokenContractAddress);
        }

        protected async Task<decimal> BalanceFulcrumInToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceFulcrumInTokenFunction function = new BalanceFulcrumInTokenFunction();
            BalanceFulcrumInTokenOutputDTO output = await _web3Client.CallContract<BalanceFulcrumInTokenOutputDTO, BalanceFulcrumInTokenFunction>(Networks.BSC, protocolContractAddress, function);
            return await ConvertBaseUnitToAmount(output.Balance, networks, protocolContractAddress);
        }

        protected async Task<decimal> BalanceFortube(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceFortubeFunction function = new BalanceFortubeFunction();
            BalanceFortubeOutputDTO output = await _web3Client.CallContract<BalanceFortubeOutputDTO, BalanceFortubeFunction>(networks, protocolContractAddress, function);
            string fortubeTokenContractAddress = await FortubeToken(asset, networks);
            return await ConvertBaseUnitToAmount(output.Balance, networks, fortubeTokenContractAddress);
        }

        protected async Task<decimal> BalanceFortubeInToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BalanceFortubeInTokenFunction function = new BalanceFortubeInTokenFunction();
            BalanceFortubeInTokenOutputDTO output = await _web3Client.CallContract<BalanceFortubeInTokenOutputDTO, BalanceFortubeInTokenFunction>(networks, protocolContractAddress, function);
            return await ConvertBaseUnitToAmount(output.Balance, networks, protocolContractAddress);
        }

        protected async Task<string> Token(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            TokenFunction function = new TokenFunction();
            TokenFunctionOutputDTO output = await _web3Client.CallContract<TokenFunctionOutputDTO, TokenFunction>(networks, protocolContractAddress, function);
            return output.Token;
        }

        protected async Task<string> FulcrumToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FulcrumFunction function = new FulcrumFunction();
            FulcrumFunctionOutputDTO output = await _web3Client.CallContract<FulcrumFunctionOutputDTO, FulcrumFunction>(networks, protocolContractAddress, function);
            return output.Token;
        }

        protected async Task<string> FortubeToken(Assets asset, Networks networks)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            FortubeTokenFunction function = new FortubeTokenFunction();
            FortubeTokenFunctionOutputDTO output = await _web3Client.CallContract<FortubeTokenFunctionOutputDTO, FortubeTokenFunction>(networks, protocolContractAddress, function);
            return output.Token;
        }

        protected async Task<decimal> GetPricePerShareAsync(Assets asset, Networks network)
        {
            string tokenContractAddress = GetAssetContractAddress(asset, network);
            BigInteger pricePerShareBaseUnit = await GetPricePerShare(asset, network);
            decimal pricePerShare = await ConvertBaseUnitToAmount(pricePerShareBaseUnit, network, tokenContractAddress);
            return pricePerShare;
        }

        protected async Task<decimal> GetWorthOfSharesAsync(string address, Assets asset, Networks network)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            BigInteger balanceInBaseUnit = await GetBalanceOf(protocolContractAddress, address, network);
            BigInteger pricePerShareInBaseUnit = await GetPricePerFullShare(asset, network);
            BigInteger shareValueInBaseUnit = BigInteger.Multiply(pricePerShareInBaseUnit, balanceInBaseUnit);
            decimal shareValue = await ConvertBaseUnitToAmount(shareValueInBaseUnit, Networks.BSC, protocolContractAddress);
            return shareValue;
        }

        protected async Task<BigInteger> GetPricePerFullShare(Assets asset, Networks network)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            GetPricePerFullShareFunction pricePerFullShareFunction = new GetPricePerFullShareFunction();
            GetPricePerFullShareOutputDTO output = await _web3Client.CallContract<GetPricePerFullShareOutputDTO, GetPricePerFullShareFunction>(network, protocolContractAddress, pricePerFullShareFunction);
            return output.Value;
        }

        protected async Task<BigInteger> ConvertAmountToBaseUnit(decimal amount, string assetContractAddress, Networks network)
        {
            BigInteger decimalPlaces = await GetDecimals(assetContractAddress, network); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
            int dp = int.Parse(decimalPlaces.ToString()); // converts string representation of BigInteger to int type
            BigInteger result = Web3.Convert.ToWei(amount, dp);
            return result;
        }

        private async Task<BigInteger> GetPricePerShare(Assets asset, Networks network)
        {
            string protocolContractAddress = GetProtocolContractAddress(asset);
            PricePerShareFunction pricePerShareFunction = new PricePerShareFunction();
            PricePerShareOutputDTO output = await _web3Client.CallContract<PricePerShareOutputDTO, PricePerShareFunction>(network, protocolContractAddress, pricePerShareFunction);
            return output.PricePerShare;
        }

        /// <summary>
        /// To be implemented by the derived classes. the base class is not meant to have an implementation as it's characteristics are unique for each chain
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        protected virtual string GetProtocolContractAddress(Assets asset)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetAssetContractAddress(Assets asset)
        {
            throw new NotImplementedException();
        }

        protected async Task<decimal> ConvertBaseUnitToAmount(BigInteger amount, Networks networks, string assetContractAddress)
        {
            BigInteger decimalPlaces = await GetDecimals(assetContractAddress, networks); // Get's decimals of tokenized asset. BUSD - 18, USDT - 6
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

        protected string GetAssetContractAddress(Assets asset, Networks network)
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

        protected string GetPolygonAssetContractAddress(Assets asset)
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

        protected string GetBSCAssetContractAddress(Assets asset)
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