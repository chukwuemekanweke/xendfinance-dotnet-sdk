require('@nomiclabs/hardhat-waffle');
require("dotenv").config();

const bscProviderUrl = process.env.BSC_MAINNET_PROVIDER_URL;
const polygonProviderUrl = process.env.POLYGON_MAINNET_PROVIDER_URL;

if (!bscProviderUrl || !polygonProviderUrl) {
  console.error('Missing JSON RPC provider URL as environment variable `MAINNET_PROVIDER_URL`');
  process.exit(1);
}

module.exports = {
  networks: {
    hardhatbsc: {
      chainId: 56,
      url: "http://localhost:8545",
      forking: {
        url: bscProviderUrl,
        blockNumber: 20406225,
      },
      gasPrice: 0,
      loggingEnabled: true,
    }
    ,
    hardhatpolygon: {
        chainId: 137,
        url: "http://localhost:8545",
        forking: {
          url: polygonProviderUrl,
          blockNumber: 20406225,
        },
        gasPrice: 0,
        loggingEnabled: true,
      },
  },
  mocha: {
    timeout: 60000
  }
};