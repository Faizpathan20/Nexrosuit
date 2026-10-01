using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexoraSuite.Models;

namespace NexoraSuite.ViewComponents
{
    public class ProductSummaryViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductSummaryViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var products = await _context.Products.ToListAsync();

            var stats = new
            {
                Total = products.Count,
                InStock = products.Count(p => p.Stock > 10),
                LowStock = products.Count(p => p.Stock > 0 && p.Stock <= 10),
                Orders = 25
            };

            return View(stats);
        }
    }
}
