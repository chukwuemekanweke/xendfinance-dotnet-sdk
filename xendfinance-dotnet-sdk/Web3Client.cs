using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Models.Enums;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk
{
    internal sealed class Web3Client : IWeb3Client
    {
        private readonly Web3 _bscWeb3;
        private readonly Web3 _polygonWeb3;
        private Account _bscAccount;
        private Account _polygonAccount;
        private GasPriceLevel _gasPriceLevel;

        public Web3Client(string privateKey, BigInteger bscChainId, BigInteger polygonChainId, string bscNodeUrl, string polygonNodeUrl, GasPriceLevel gasPriceLevel)
        {
            _bscAccount = new Account(privateKey, bscChainId);
            _polygonAccount = new Account(privateKey, polygonChainId);
            _bscWeb3 = new Web3(_bscAccount, bscNodeUrl);
            _polygonWeb3 = new Web3(_polygonAccount, polygonNodeUrl);
            _gasPriceLevel = gasPriceLevel;
        }

        public async Task<IEnumerable<EventLog<TEventMessage>>> GetEvents<TEventMessage>(Networks network, string contractAddress, ulong startBlock, ulong endBlock) where TEventMessage : IEventDTO, new()
        {
            BlockParameter startBlockParameter = new BlockParameter(startBlock);
            BlockParameter endBlockParameter = new BlockParameter(endBlock);
            Web3 web3 = GetWeb3Instance(network);
            Event<TEventMessage> handler = web3.Eth.GetEvent<TEventMessage>(contractAddress);
            NewFilterInput filterInput = handler.CreateFilterInput(startBlockParameter, endBlockParameter);

            IEnumerable<EventLog<TEventMessage>> events = await handler.GetAllChangesAsync(filterInput);
            return events;
        }

        public async Task<ulong> GetLatestBlock(Networks network)
        {
            Web3 web3 = GetWeb3Instance(network);
            HexBigInteger blockNumberHex = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return ulong.Parse(blockNumberHex.Value.ToString());
        }

        public async Task<ulong> GetBlockTimeStamp(Networks network, ulong blockNumber)
        {
            Web3 web3 = GetWeb3Instance(network);
            BlockParameter blockParameter = new BlockParameter(blockNumber);
            BlockWithTransactions blockWithTransactions = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockParameter);
            ulong blockTimeStamp = ulong.Parse(blockWithTransactions.Timestamp.Value.ToString());
            return blockTimeStamp;
        }

        public async Task<string> SendTransactionAsync(Networks network, string contractAddress, string abi, string functionName, GasPriceLevel? gasPriceLevel, params object[] functionInput)
        {           
            Contract contract = GetContract(network, contractAddress, abi);
            Account account = GetAccountInstance(network);
            var function = contract.GetFunction(functionName);
            var gas =await function.EstimateGasAsync(functionInput);
            string transactionHash = await function.SendTransactionAsync(account.Address, gas, null, null, functionInput);
            return transactionHash;
        }

        public async Task<T> CallContract<T>(Networks network, string contractAddress, string abi, string functionName, params object[] functionInput) where T : IFunctionOutputDTO, new()
        {
            Contract contract = GetContract(network, contractAddress, abi);
            var function = contract.GetFunction(functionName);
            T response = await function.CallDeserializingToObjectAsync<T>(functionInput);
            return response;
        }

        public async Task<T> CallContract<T>(Networks network, string contractAddress, string abi, string functionName, string from, params object[] functionInput) where T : IFunctionOutputDTO, new()
        {
            Contract contract = GetContract(network, contractAddress, abi);
            var function = contract.GetFunction(functionName);
            T response = await function.CallDeserializingToObjectAsync<T>(from, null, null, functionInput);
            return response;
        }

        public async Task<T> CallContract<T, W>(Networks network, string contractAddress, W inputFunction) where W : FunctionMessage, new() where T : class, new()
        {
            Web3 web3 = GetWeb3Instance(network);
            ContractHandler contractHandler = web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<W, T>(inputFunction);
        }

        private Contract GetContract(Networks network, string contractAddress, string abi)
        {
            contractAddress = AddressValidator.ValidateAddress(contractAddress);
            Web3 web3 = GetWeb3Instance(network);
            Contract contract = web3.Eth.GetContract(abi, contractAddress);
            return contract;
        }

        private Account GetAccountInstance(Networks network)
        {
            switch (network)
            {
                case Networks.BSC:
                    return _bscAccount;
                case Networks.POLYGON:
                    return _polygonAccount;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported Network Chain");
            }
        }

        private Web3 GetWeb3Instance(Networks network)
        {
            switch (network)
            {
                case Networks.BSC:
                    return _bscWeb3;
                case Networks.POLYGON:
                    return _polygonWeb3;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported Network Chain");
            }
        }
    }
}
