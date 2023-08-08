using Microsoft.AspNetCore.Mvc;
namespace NeftViewer.MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {



            //using var channel = GrpcChannel.ForAddress("https://localhost:32824");
            //var client = new Greeter.GreeterClient(channel);

            //var reply = client.SayHelloAsync(new HelloRequest { Name = "TestResult" });
            //string str = reply.ToJson();


            return View();
        }
    }
}
