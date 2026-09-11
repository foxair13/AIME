// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using NeftViewer.SV.Services;
using System.Configuration;

class Program
{
    static void Main(string[] args)
    {
        
        IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json")
    .Build();
        CryptoService cre = new CryptoService(configuration);
        if (args.Length < 2)
        {
            Console.WriteLine("Пароль не может быть меньше двух символов");
            return;
        }
        string password = args[0];
        string command = args[1].ToLower();
        string hid = args[2];
        if (password != "7f4df451")
        {
            Console.WriteLine("Доступ запрещен....");
            return;
        }

        switch (command)
        {
            case "encrypt":
                cre.EncryptConnectionsInConfig();
                break;

            case "hit":
                cre.HitConnectionsInConfig();
                break;

            default:
                Console.WriteLine("Invalid command. Usage: dotnet YourProgram.dll <password> [encrypt|hit]");
                break;
        }

        IConfiguration tokenconfiguration = cre.GetConfiguration();
        var connectionsSection = tokenconfiguration.GetSection("Connections");
        foreach (var connection in connectionsSection.GetChildren())
        {

            Console.WriteLine(connection.Key + ": " + connection.Value);
        }
     
        CryptoService cs = new CryptoService(cre.GetConfiguration());
        if (hid != "")
        {
            Console.WriteLine("ID: " + CryptoService.GetToken());
            Console.WriteLine("Ключ: " + cs.Encrypt(hid));
        }
        else 
        {
            Console.WriteLine("ID: " + CryptoService.GetToken());
            Console.WriteLine("Ключ: " + cs.Encrypt(CryptoService.GetToken()));
        }
       
        Console.ReadLine();
    }
}