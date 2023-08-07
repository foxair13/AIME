using Microsoft.Extensions.Options;
using NeftViewer.MVC.Options;

namespace NeftViewer.MVC
{
    public static class AppConfig
    {
      
        private static  Connections _connections;
        public static void Initialize(Connections connections)
        {
            _connections = connections;
        }

        public static Connections GetConnectionString()
        {
            return _connections;
        }
    }
}
