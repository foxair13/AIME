using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Repositories.EntityRepositories
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        private readonly NeftViewerContext _dbContext;

        public CustomerRepository(NeftViewerContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

   
    }
}
