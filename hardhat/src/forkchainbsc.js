const ERC20ABI = require("../abi/erc20abi.json")


const { TASK_NODE_CREATE_SERVER } = require("hardhat/builtin-tasks/task-names");
const hre = require('hardhat');
const { ethers } = require("hardhat");
const resetForkedChain  = require("resetForkedChain")

const ethers = hre.ethers;

const mnemonic = hre.network.config.accounts.mnemonic;
const addresses = [];
const privateKeys = [];

const busdAccountToInpersonate = "0x8894E0a0c962CB723c1976a4421c95949bE2D4E3"
const usdcAccountToInpersonate = "0x8894E0a0c962CB723c1976a4421c95949bE2D4E3"
const usdtAccountToInpersonate = "0x8894E0a0c962CB723c1976a4421c95949bE2D4E3"

const BUSD_CONTRACT_ADDRESS = "0xe9e7CEA3DedcA5984780Bafc599bD69ADd087D56";
const USDT_CONTRACT_ADDRESS = "0x55d398326f99059fF775485246999027B3197955";
const USDC_CONTRACT_ADDRESS = "0x8AC76a51cc950d9822D68b83fE1Ad97B32Cd580d";


for (let i = 0; i < 3; i++) {
  const wallet = new ethers.Wallet.fromMnemonic(mnemonic, `m/44'/60'/0'/0/${i}`);
  addresses.push(wallet.address);
  privateKeys.push(wallet._signingKey().privateKey);
}

// async function StartJsonRPCServer()
// {
//   let jsonRpcServer = await hre.run(TASK_NODE_CREATE_SERVER, {
//     hostname: 'localhost',
//     port: 8545,
//     provider: hre.network.provider
//   });

//   await jsonRpcServer.listen();

//   return jsonRpcServer;
// }

async function ImpersonateAccount() {
  await hre.network.provider.request({
    method: "hardhat_impersonateAccount",
    params: [busdAccountToInpersonate, usdcAccountToInpersonate, usdtAccountToInpersonate],
  });

  const busdSigner = await ethers.getSigner(busdAccountToInpersonate)
  const usdcSigner = await ethers.getSigner(usdcAccountToInpersonate)
  const usdtSigner = await ethers.getSigner(usdtAccountToInpersonate)

  const busdContract = new ethers.Contract(BUSD_CONTRACT_ADDRESS, ERC20ABI, busdSigner);
  const usdcContract = new ethers.Contract(USDC_CONTRACT_ADDRESS, ERC20ABI, usdcSigner);
  const usdtContract = new ethers.Contract(USDT_CONTRACT_ADDRESS, ERC20ABI, usdtSigner);


  const busdBalance = await busdContract.balanceOf(busdAccountToInpersonate);
  const usdcBalance = await usdcContract.balanceOf(usdcAccountToInpersonate);
  const usdtBalance = await usdtContract.balanceOf(usdtAccountToInpersonate);

  console.log("whale busd balance", busdBalance / 1e18)
  console.log("whale usdc balance", usdcBalance / 1e18)
  console.log("whale usdt balance", usdtBalance / 1e18)


  await busdContract.connect(busdSigner).transfer(addresses[0], busdBalance)
  const busdBalance2 = await busdContract.balanceOf(addresses[0])

  await usdcContract.connect(usdcSigner).transfer(addresses[0], usdcBalance)
  const usdcBalance2 = await usdcContract.balanceOf(addresses[0])

  await usdtContract.connect(usdtSigner).transfer(addresses[0], usdtBalance)
  const usdtBalance2 = await usdtContract.balanceOf(addresses[0])

  console.log("custodial busd balance", busdBalance2 / 1e18)
  console.log("custodial usdc balance", usdcBalance2 / 1e18)
  console.log("custodial usdt balance", usdtBalance2 / 1e18)
}