using Grpc.Core;
using NeftViewer.Grpc;

namespace NeftViewer.Grpc.Services
{
    public class MigrationService : Migrate.MigrateBase
    {
        private readonly ILogger<MigrationService> _logger;
        public MigrationService(ILogger<MigrationService> logger)
        {
            _logger = logger;
        }

        public override Task<Reply> FinanceService(Request request, ServerCallContext context)
        {
            bool flag = true;
            switch (request.Param)
            {
                case "":

                    break;

                default:

                    break;
            }
            return Task.FromResult(new Reply
            {
                Migrationresult = flag
            });
        }
        public void DoMigration(object state)
        {
        }
    }
}