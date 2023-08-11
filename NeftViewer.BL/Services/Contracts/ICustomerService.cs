using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services.Contracts
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> FindCustomerAsync(string? id);
        EntityEntry<Customer> UpdateCustomer(Customer customer);
        Task CommitChangesAsync();
        EntityEntry<Customer> DeleteCustomer(string id);
        Task<bool> AddCustomer(Customer customer);
        Task<bool> AddCustomerRange(IEnumerable<Customer> customers);
    }
}
