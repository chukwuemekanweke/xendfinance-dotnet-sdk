using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xendfinance_dotnet_sdk.Interfaces;

namespace xendfinance_dotnet_sdk.Services.XAuto
{
    public sealed class XAutoPolygonConnectorService : ERC20Primitive
    {
        private readonly IWeb3Client _web3Client;

        public XAutoPolygonConnectorService(IWeb3Client web3Client) : base(web3Client)
        {
            _web3Client = web3Client;

        }
    }
}
