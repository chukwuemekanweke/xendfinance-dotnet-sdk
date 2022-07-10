using System.Numerics;
using xendfinance_dotnet_sdk.Functions;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.OutputDTOs.Shared;

namespace xendfinance_dotnet_sdk.Services
{
    public abstract class ERC20Primitive
    {
        private readonly IWeb3Client _web3Client;

        public ERC20Primitive(IWeb3Client web3Client)
        {
            _web3Client = web3Client;
        }

        protected async Task<BigInteger> GetDecimals(string assetContractAddress, Networks networks)
        {
            DecimalsFunction decimalsFunction = new DecimalsFunction();
            DecimalsOutputDTO output = await _web3Client.CallContract<DecimalsOutputDTO, DecimalsFunction>(networks, assetContractAddress, decimalsFunction);
            return output.Decimals;
        }

        protected async Task<BigInteger> GetBalanceOf(string contractAddress, string address, Networks network)
        {
            BalanceOfFunction function = new BalanceOfFunction()
            {
                Address = address
            };
            BalanceOfOutputDTO output = await _web3Client.CallContract<BalanceOfOutputDTO, BalanceOfFunction>(network, contractAddress, function);
            BigInteger balanceInBaseUnit = output.Balance;
            return balanceInBaseUnit;
        }
    }
}