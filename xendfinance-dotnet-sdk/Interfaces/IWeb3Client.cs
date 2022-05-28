﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.Interfaces
{
    public interface IWeb3Client
    {
        Task<ulong> GetBlockTimeStamp(Networks network,ulong blockNumber);
        Task<ulong> GetLatestBlock(Networks network);
        Task<IEnumerable<EventLog<TEventMessage>>> GetEvents<TEventMessage>(Networks network, string contractAddress, ulong startBlock, ulong endBlock) where TEventMessage : IEventDTO, new();
        Task<T> CallContract<T>(Networks network, string contractAddress, string abi, string functionName, params object[] functionInput) where T : IFunctionOutputDTO, new();
        Task<T> CallContract<T>(Networks network, string contractAddress, string abi, string functionName, string from, params object[] functionInput) where T : IFunctionOutputDTO, new();
        Task<T> CallContract<T, W>(Networks network, string contractAddress, W inputFunction) where W : FunctionMessage, new() where T : class, new();
    }
}
