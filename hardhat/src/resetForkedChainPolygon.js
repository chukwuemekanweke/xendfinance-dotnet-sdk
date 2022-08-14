const hre = require('hardhat');

async function resetForkedChain() {
  const forkUrl = hre.config.networks.hardhatpolygon.forking.url;
  const forkBlockNumber = hre.config.networks.hardhatpolygon.forking.blockNumber;
  await hre.network.provider.request({
    method: 'hardhat_reset',
    params: [{
      forking: {
        jsonRpcUrl: forkUrl,
        blockNumber: forkBlockNumber
      }
    }]
  });

}