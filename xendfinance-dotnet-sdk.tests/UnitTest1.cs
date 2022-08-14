using xendfinance_dotnet_sdk.tests.Setup;
using Xunit.Abstractions;

namespace xendfinance_dotnet_sdk.tests
{
    public class UnitTest1:IClassFixture<BSCContractEnvironmentFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            _testOutputHelper.WriteLine("Test 1 Run To Test Script To Run Harhat");
            Assert.True(true);
        }
    }
}