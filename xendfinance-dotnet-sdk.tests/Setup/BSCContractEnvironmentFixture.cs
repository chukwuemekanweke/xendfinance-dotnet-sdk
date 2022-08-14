using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.tests.Setup
{
    public class BSCContractEnvironmentFixture : IDisposable
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public BSCContractEnvironmentFixture()
        {
            HardhatEnvioronmentSetup.StartupHardHatEnvironment(Networks.BSC).GetAwaiter().GetResult();
        }
    }
}
