using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace xendfinance_dotnet_sdk.Interfaces
{
    public interface IWeb3Client
    {
        Task<ulong> GetBlockTimeStamp(ulong blockNumber);
        Task<ulong> GetLatestBlock();
        Task<IEnumerable<EventLog<TEventMessage>>> GetEvents<TEventMessage>(string contractAddress, ulong startBlock, ulong endBlock) where TEventMessage : IEventDTO, new();
        Task<T> CallContract<T>(string contractAddress, string abi, string functionName, params object[] functionInput) where T : IFunctionOutputDTO, new();
        Task<T> CallContract<T>(string contractAddress, string abi, string functionName, string from, params object[] functionInput) where T : IFunctionOutputDTO, new();
        Task<T> CallContract<T, W>(string contractAddress, W inputFunction) where W : FunctionMessage, new() where T : class, new();
    }
}
