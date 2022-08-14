using xendfinance_dotnet_sdk.Models.Enums;

namespace xendfinance_dotnet_sdk.tests.Setup
{
    public class PolygonContractEnvironmentFixture: IDisposable
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public PolygonContractEnvironmentFixture()
        {
            HardhatEnvioronmentSetup.StartupHardHatEnvironment(Networks.BSC).GetAwaiter().GetResult();
        }
    }
}
