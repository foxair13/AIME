using NeftViewer.Core.ActionFilters;
using NeftViewer.MVC.Options;
using NeftViewer.SV.Services;

namespace NeftViewer.MVC.Service
{
    public static class GetConnections
    {
     
        public static Connections Approved(String Prot, String proc, CryptoService cryptoService)
        {

            if (CheckToken(Prot, proc))
            {
                return ApplyAdditional(cryptoService, Prot);
            }
            return HandleInvalid();
        }

        private static string Hit(String input)
        {
            var builder = WebApplication.CreateBuilder();
            CryptoService cs = new CryptoService(builder.Configuration);
            return cs.Hit(input);
        }

        private static bool CheckToken(string token, string referenceToken)
        {
            string tok = Hit(token);
            return tok == referenceToken;
        }
        private static Connections ApplyAdditional(CryptoService cryptoService, String Prot)
        {
            CustomAuthorizeAttribute.SetResult(true);
         
            var connections = new Connections(Prot)
            {
                BasePostgree = cryptoService.GetConfiguration().GetSection("Connections:BasePostgree").Value,
                FinanceMssql = cryptoService.GetConfiguration().GetSection("Connections:FinanceMssql").Value,
                CoordsUrl = cryptoService.GetConfiguration().GetSection("Connections:CoordsUrl").Value,
                AsuUrl = cryptoService.GetConfiguration().GetSection("Connections:AsuUrl").Value
            };
            return connections;
        }

        private static Connections HandleInvalid()
        {
            CustomAuthorizeAttribute.SetResult(false);
            return null;
        }
    }
}
