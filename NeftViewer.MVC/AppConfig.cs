using Microsoft.Extensions.Options;
using NeftViewer.MVC.Options;
using NeftViewer.MVC.Service;
using NeftViewer.SV.Services;
using NuGet.Common;

namespace NeftViewer.MVC
{
    public static class AppConfig
    {

        private static Connections _connections;

        public static void Initialize(CryptoService cryptoService, String Prot)
        {
            _connections = GetConnections.Approved(Prot, CryptoService.GetToken(), cryptoService);
        }
       

        public static Connections GetConnectionString()
        {
            return _connections;
        }
    }
}
