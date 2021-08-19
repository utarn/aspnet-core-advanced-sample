using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using RedisWebDay3.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RedisWebDay3.Data;
using RedisWebDay3.Repository;

namespace RedisWebDay3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerRepository _repository;
        private readonly ApplicationDbContext _context;
        private readonly ICacheProvider _cacheProvider;
        public HomeController(ILogger<HomeController> logger, ICustomerRepository repository, ApplicationDbContext context, ICacheProvider cacheProvider)
        {
            _logger = logger;
            _repository = repository;
            _context = context;
            _cacheProvider = cacheProvider;
        }

        public async Task<IActionResult> Index()
        {
            await GetCustomerFromCache();
            return View();
        }

        private async Task GetCustomerFromCache()
        {
            var cached = await _cacheProvider.GetFromCache<List<Customer>>("current_customer");
            if (cached != null)
            {
                ViewData["Customers"] = cached;
            }
            else
            {
                var customers = await _context.Customers.ToListAsync();
                await _cacheProvider.SetCache("current_customer", customers, new DistributedCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
                ViewData["Customers"] = customers;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(Customer customer)
        {
            await _repository.WritePendingCustomer(customer);
            await GetCustomerFromCache();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
