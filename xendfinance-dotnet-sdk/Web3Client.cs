using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System.Numerics;
using xendfinance_dotnet_sdk.Interfaces;
using xendfinance_dotnet_sdk.Utilities;

namespace xendfinance_dotnet_sdk
{
    public sealed class Web3Client : IWeb3Client
    {
        private Web3 Web3;
        private readonly string NodeUrl;

        public Web3Client(string nodeUrl)
        {
            NodeUrl = nodeUrl;
        }

        public async Task<IEnumerable<EventLog<TEventMessage>>> GetEvents<TEventMessage>(string contractAddress, ulong startBlock, ulong endBlock) where TEventMessage : IEventDTO, new()
        {
            InitializeWeb3();
            BlockParameter startBlockParameter = new BlockParameter(startBlock);
            BlockParameter endBlockParameter = new BlockParameter(endBlock);

            Event<TEventMessage> handler = Web3.Eth.GetEvent<TEventMessage>(contractAddress);
            NewFilterInput filterInput = handler.CreateFilterInput(startBlockParameter, endBlockParameter);

            IEnumerable<EventLog<TEventMessage>> events = await handler.GetAllChangesAsync(filterInput);
            return events;
        }

        public async Task<ulong> GetLatestBlock()
        {
            InitializeWeb3();

            HexBigInteger blockNumberHex = await Web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            return ulong.Parse(blockNumberHex.Value.ToString());
        }

        public async Task<ulong> GetBlockTimeStamp(ulong blockNumber)
        {
            InitializeWeb3();

            BlockParameter blockParameter = new BlockParameter(blockNumber);
            BlockWithTransactions blockWithTransactions = await Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockParameter);
            ulong blockTimeStamp = ulong.Parse(blockWithTransactions.Timestamp.Value.ToString());
            return blockTimeStamp;
        }

        public async Task<T> CallContract<T>(string contractAddress, string abi, string functionName, params object[] functionInput) where T : IFunctionOutputDTO, new()
        {
            Contract contract = GetContract(contractAddress, abi);
            var function = contract.GetFunction(functionName);
            T response = await function.CallDeserializingToObjectAsync<T>(functionInput);
            return response;
        }

        public async Task<T> CallContract<T>(string contractAddress, string abi, string functionName, string from, params object[] functionInput) where T : IFunctionOutputDTO, new()
        {
            Contract contract = GetContract(contractAddress, abi);
            var function = contract.GetFunction(functionName);
            T response = await function.CallDeserializingToObjectAsync<T>(from, null, null, functionInput);
            return response;
        }

        public async Task<T> CallContract<T, W>(string contractAddress, W inputFunction) where W : FunctionMessage, new() where T : class, new()
        {
            InitializeWeb3();
            ContractHandler contractHandler = Web3.Eth.GetContractHandler(contractAddress);
            return await contractHandler.QueryAsync<W, T>(inputFunction);
        }

        private void InitializeWeb3()
        {
            if (Web3 == null)
            {
                Web3 = new Web3(url: NodeUrl);
            }
        }

        private Contract GetContract(string contractAddress, string abi)
        {
            contractAddress = AddressValidator.ValidateAddress(contractAddress);
            Web3 web3 = new Web3(NodeUrl);
            Contract contract = web3.Eth.GetContract(abi, contractAddress);
            return contract;
        }
    }
}
