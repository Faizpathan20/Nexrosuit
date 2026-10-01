using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NexoraSuite.Models;
using System;

namespace NexoraSuite.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.Include(c => c.DeliveryAddresses).ToListAsync();
            return View(customers);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.DeliveryAddresses)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null) return NotFound();

            return Json(new
            {
                customerId = customer.CustomerId,
                name = customer.Name,
                address = customer.Address,
                phone = customer.Phone,
                customerType = customer.CustomerType,
                businessStart = customer.BusinessStart.ToString("yyyy-MM-dd"),
                email = customer.Email,
                creditLimit = customer.CreditLimit,
                photo = customer.Photo,
                deliveryAddresses = customer.DeliveryAddresses.Select(d => new {
                    address = d.Address,
                    phone = d.Phone,
                    contactPerson = d.ContactPerson
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Customer customer, IFormFile? photo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (photo != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    var fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }
                    customer.Photo = fileName;
                }

                var pName = new SqlParameter("@Name", customer.Name);
                var pAddr = new SqlParameter("@Address", customer.Address);
                var pDate = new SqlParameter("@BusinessStart", customer.BusinessStart);
                var pPhone = new SqlParameter("@Phone", customer.Phone);
                var pType = new SqlParameter("@CustomerType", customer.CustomerType);
                var pEmail = new SqlParameter("@Email", customer.Email);
                var pLimit = new SqlParameter("@CreditLimit", customer.CreditLimit);
                var pPhoto = new SqlParameter("@Photo", (object)customer.Photo ?? DBNull.Value);

                if (customer.CustomerId > 0)
                {
                    var pId = new SqlParameter("@CustomerId", customer.CustomerId);
                    await _context.Database.ExecuteSqlRawAsync("EXEC spCustomerUpdate @CustomerId, @Name, @Address, @BusinessStart, @Phone, @CustomerType, @Email, @CreditLimit, @Photo",
                        pId, pName, pAddr, pDate, pPhone, pType, pEmail, pLimit, pPhoto);

                    var oldAddrs = _context.DeliveryAddresses.Where(x => x.CustomerId == customer.CustomerId);
                    _context.DeliveryAddresses.RemoveRange(oldAddrs);
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC spCustomerInsert @Name, @Address, @BusinessStart, @Phone, @CustomerType, @Email, @CreditLimit, @Photo",
                        pName, pAddr, pDate, pPhone, pType, pEmail, pLimit, pPhoto);
                }

                await _context.SaveChangesAsync();
                int currentId = customer.CustomerId > 0 ? customer.CustomerId : await _context.Customers.MaxAsync(c => c.CustomerId);

                if (customer.DeliveryAddresses != null)
                {
                    foreach (var addr in customer.DeliveryAddresses)
                    {
                        addr.CustomerId = currentId;
                        _context.DeliveryAddresses.Add(addr);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var pId = new SqlParameter("@CustomerId", id);
            await _context.Database.ExecuteSqlRawAsync("EXEC spCustomerDelete @CustomerId", pId);
            return Json(new { redirectTo = Url.Action("Index") });
        }
    }
}