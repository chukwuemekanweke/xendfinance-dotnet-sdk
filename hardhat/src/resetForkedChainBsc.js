const hre = require('hardhat');
require("dotenv").config();

async function resetForkedChain() {
  const forkUrl = hre.config.networks.hardhatbsc.forking.url;
  const forkBlockNumber = hre.config.networks.hardhatbsc.forking.blockNumber;
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


resetForkedChain()
.then(x=>{

})
.catch(err=>{
  console.log({err})
});
