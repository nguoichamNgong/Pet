using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Pet.Constants;
using Pet.Repository;
using Pet.Models.ViewModels;

namespace Pet.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _dataContext.Orders.OrderByDescending(p => p.Id).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> ViewOrder(string orderCode)
        {
            if (string.IsNullOrEmpty(orderCode))
            {
                return BadRequest("Mã đơn hàng là bắt buộc.");
            }

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            if (order == null)
            {
                return NotFound($"Không tìm thấy đơn hàng với mã: {orderCode}");
            }

            var orderDetails = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderCode == orderCode)
                .ToListAsync();

            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound($"Không tìm thấy chi tiết đơn hàng cho mã đơn hàng: {orderCode}");
            }

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            var order = await _dataContext.Orders.FindAsync(Id);
            if (order == null)
            {
                return NotFound();
            }

            _dataContext.Orders.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["success"] = "Đơn hàng đã được xóa";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MarkAsProcessed(int id)
        {
            var order = _dataContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = 0;
            _dataContext.SaveChanges();

            return Ok();
        }

    }
}
