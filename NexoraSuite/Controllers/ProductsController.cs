using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NexoraSuite.Models;
using System;

namespace NexoraSuite.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                var pName = new SqlParameter("@Name", product.Name);
                var pCat = new SqlParameter("@Category", (object)product.Category ?? DBNull.Value);
                var pSKU = new SqlParameter("@SKU", (object)product.SKU ?? DBNull.Value);
                var pPrice = new SqlParameter("@Price", product.Price);
                var pStock = new SqlParameter("@Stock", product.Stock);
                var pDesc = new SqlParameter("@Description", (object)product.Description ?? DBNull.Value);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC spProductInsert @Name, @Category, @SKU, @Price, @Stock, @Description",
                    pName, pCat, pSKU, pPrice, pStock, pDesc);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return View(product);
            }
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            var pId = new SqlParameter("@ProductId", product.ProductId);
            var pName = new SqlParameter("@Name", product.Name);
            var pCat = new SqlParameter("@Category", (object)product.Category ?? DBNull.Value);
            var pSKU = new SqlParameter("@SKU", (object)product.SKU ?? DBNull.Value);
            var pPrice = new SqlParameter("@Price", product.Price);
            var pStock = new SqlParameter("@Stock", product.Stock);
            var pDesc = new SqlParameter("@Description", (object)product.Description ?? DBNull.Value);

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC spProductUpdate @ProductId, @Name, @Category, @SKU, @Price, @Stock, @Description",
                pId, pName, pCat, pSKU, pPrice, pStock, pDesc);

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var pId = new SqlParameter("@ProductId", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC spProductDelete @ProductId", pId);
            return RedirectToAction(nameof(Index));
        }
    }
}