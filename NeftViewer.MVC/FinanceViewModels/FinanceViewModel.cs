using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.FinanceModels
{
    public class FinanceViewModel : DbContext
    {
        private readonly NeftViewerContext _context;
        
        public FinanceViewModel(NeftViewerContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
