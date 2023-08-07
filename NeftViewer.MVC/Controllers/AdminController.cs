using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

using NuGet.Protocol;

namespace NeftViewer.MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            //try
            //{
            //    using var channel = GrpcChannel.ForAddress("https://localhost:32786/");
            //    var client = new Migrate.MigrateClient(channel);
            //    var reply = client.FinanceServiceAsync(new Request {  = "GreeterClient" });
            //    string str = reply.ToJson();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}

            return View();
        }
    }
}
