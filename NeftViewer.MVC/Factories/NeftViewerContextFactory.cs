using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;

namespace NeftViewer.MVC.Factories
{
    public static class NeftViewerContextFactory
    {
        public static NeftViewerContext CreateDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<NeftViewerContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new NeftViewerContext(options);
        }
    }
}
