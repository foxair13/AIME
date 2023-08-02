using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using NeftViewer.RPC;
using NuGet.Protocol;

namespace NeftViewer.MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:32786/");
                var client = new Greeter.GreeterClient(channel);
                var reply = client.MigrateTableAsync(new MigrateRequest { Name = "GreeterClient" });
                string str = reply.ToJson();
            }
            catch (Exception ex)
            {

                throw;
            }
           
            return View();
        }
    }
}
