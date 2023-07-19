using NeftViewer.Data.Models;
using NeftViewer.Data.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.UnitOfWork.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<AspNetUsers> AspNetUsers { get; }
        Task CommitAsync();
    }
}
