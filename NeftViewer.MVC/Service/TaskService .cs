using NeftViewer.Data.DataContext;
using NeftViewer.MVC.FinanceModels;
using Org.BouncyCastle.Utilities.Collections;

namespace NeftViewer.MVC.Service
{
    public class TaskService : BackgroundService
    {
        private readonly FinanceViewerContext fvc;
        String _connectionString = "";

        public TaskService()
        {
            _connectionString = AppConfig.GetConnectionString().FinanceMssql;
            fvc = new FinanceViewerContext(_connectionString);
        }
        private readonly TimeSpan dailyInterval = TimeSpan.FromSeconds(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool isDbConnectionSuccessful = fvc.Database.CanConnect();
                if (isDbConnectionSuccessful)
                {
                  
                }
                else
                {
                    
       
                }
                var objects= from x in fvc.Criterias
                             select x;
                foreach (var item in objects)
                {
                    var b = item;
                }
                await Task.Delay(dailyInterval, stoppingToken);
            }
        }
    }

}
