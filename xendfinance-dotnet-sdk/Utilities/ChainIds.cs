using System.Numerics;

namespace xendfinance_dotnet_sdk.Utilities
{
    internal static class ChainIds
    {
        public static BigInteger BSCMainnet
        { get { return new BigInteger(56); } }

        public static BigInteger PolygonMainnet
        { get { return new BigInteger(137); } }

        public static BigInteger BSCTestnet
        { get { return new BigInteger(97); } }

        public static BigInteger PolygonTestnet
        { get { return new BigInteger(80001); } }
    }
}