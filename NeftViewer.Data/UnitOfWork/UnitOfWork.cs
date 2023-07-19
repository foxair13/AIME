using DocumentFormat.OpenXml.Spreadsheet;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using NeftViewer.Data.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly NeftViewerContext _context;

        public UnitOfWork(
            NeftViewerContext context,
            IGenericRepository<AspNetUsers> AspNetUsersRepository
            )
        {
            _context = context;
            AspNetUsers = AspNetUsersRepository;
        }
        public IGenericRepository<AspNetUsers> AspNetUsers { get; }



        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
