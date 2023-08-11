using Microsoft.EntityFrameworkCore.ChangeTracking;
using NeftViewer.BL.Services.Contracts;
using NeftViewer.Data.Models;
using NeftViewer.Data.UnitOfWork.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.BL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _uow;
        public CustomerService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task CommitChangesAsync()
        {
            await _uow.CommitAsync();
        }

        public Task<Customer> FindCustomerAsync(string? id)
        {
            return _uow.Customers.GetAsync(id);
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {

            return await _uow.Customers.GetAllAsync();

        }

        public EntityEntry<Customer> UpdateCustomer(Customer customer)
        {
            return _uow.Customers.Update(customer);
        }
        public async Task<bool> AddCustomer(Customer customer)
        {
            bool flag = false;
            try
            {
                await _uow.Customers.Add(customer);
                flag = true;
            }
            catch
            {
                flag = false;
            }
            return flag;
        }


        public EntityEntry<Customer> DeleteCustomer(string id)
        {
            var res = _uow.Customers.DeleteByStringID(id);
            return res;
        }

        public async Task<bool> AddCustomerRange(IEnumerable<Customer> customers)
        {
            var res = await _uow.Customers.AddRange(customers);
            return res;
        }
    }
}
