using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexoraSuite.Models;
using System.Threading.Tasks;

namespace NexoraSuite.ViewComponents
{
    public class CustomerSummaryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CustomerSummaryViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Logic: Get total count and total credit limit sum
            var totalCustomers = await _context.Customers.CountAsync();
            var totalCredit = await _context.Customers.SumAsync(c => c.CreditLimit);

            ViewBag.TotalCount = totalCustomers;
            ViewBag.TotalCredit = totalCredit;

            return View();
        }
    }
}
