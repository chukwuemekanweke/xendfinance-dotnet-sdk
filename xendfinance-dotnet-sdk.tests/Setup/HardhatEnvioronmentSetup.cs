using CliWrap;
using CliWrap.Buffered;
using System.Diagnostics;
using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.tests.Setup
{
    public static class HardhatEnvioronmentSetup
    {
        /// <summary>
        /// This method runs a command on the command line that utilizes hardhat to run a local RPC node against the main network
        /// </summary>
        /// <returns></returns>
        public static async Task StartupHardHatEnvironment(Networks network)
        {
            switch (network)
            {
                case Networks.BSC:
                    await ForkMainnet("hardhat-bsc.sh");
                    break;
                case Networks.POLYGON:
                    await ForkMainnet("hardhat-polygon.sh");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported Network");
            }
        }

        private static async Task ForkMainnet(string scriptName)
        {
            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "BashScripts", scriptName);
            //var result = await Cli.Wrap($"{scriptPath}")
            //                      .ExecuteBufferedAsync();

            Process cmd = new Process();
            cmd.StartInfo.FileName = "/bin/bash";
            cmd.StartInfo.Arguments = "/bin/bash";

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = false;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine($"{scriptPath}");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string output = cmd.StandardOutput.ReadToEnd();
            Console.WriteLine(output);

        }
    }
}
