using System;
using JobSchedule.Data;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using RedisReadCacheDay3.Models;

using System.Diagnostics;
using System.Threading.Tasks;
using Hangfire;
using JobSchedule.Services;

namespace JobSchedule.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _customerService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ICustomerService customerService)
        {
            _logger = logger;
            _context = context;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Customers.ToListAsync();
            return View(model);
        }

        public IActionResult Privacy()
        {
            BackgroundJob.Enqueue(() => _customerService.AddNewCustomer(new Customer()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            }));
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
