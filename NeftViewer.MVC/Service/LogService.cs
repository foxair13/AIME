using NeftViewer.SV.Services;

namespace NeftViewer.MVC.Service
{
    public class LogService
    {
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
        }

        public void GETHID()
        {
            _logger.LogInformation(CryptoService.GetToken());
        }
    }
}
