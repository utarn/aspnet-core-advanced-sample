using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobSchedule.Data;

namespace JobSchedule.Services
{

    public interface ICustomerService
    {
        Task AddNewCustomer(Customer model);
    }
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewCustomer(Customer model)
        {
            await _context.Customers.AddAsync(model);
            await _context.SaveChangesAsync();
        }
    }
}
