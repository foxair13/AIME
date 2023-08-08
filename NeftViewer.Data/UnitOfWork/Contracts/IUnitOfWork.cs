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
        IGenericRepository<AspNetUser> AspNetUsers { get; }
        IGenericRepository<Models.Action> Action { get; }
        IGenericRepository<ActionRole> ActionRole { get; }
        IGenericRepository<Criteria> Criterias { get; }
        Task CommitAsync();
    }
}
