using Grpc.Core;
using NeftViewer.RPC;

namespace NeftViewer.RPC.Services
{
    public class MigrationService : Greeter.GreeterBase
    {
        private readonly ILogger<MigrationService> _logger;
        public MigrationService(ILogger<MigrationService> logger)
        {
            _logger = logger;
        }

        public override Task<MigrateReply> MigrateTable(MigrateRequest request, ServerCallContext context)
        {
         
            return Task.FromResult(new MigrateReply
            {
               Message= request.Name
            });
        }
    }
}