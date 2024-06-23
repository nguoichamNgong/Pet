using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Pet.Models;
using Pet.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Pet.Repository;

namespace Pet.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly PaypalClient _paypalClient;
        private readonly DataContext _dataContext;

        public CheckoutController(DataContext context, PaypalClient paypalClient)
        {
            _paypalClient = paypalClient;
            _dataContext = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            decimal grandTotal = cartItems.Sum(item => item.Price * item.Quantity);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                GrandTotal = grandTotal
            };
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            if (ModelState.IsValid)
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = userEmail,
                    Status = 1,
                    CreateDate = DateTime.Now
                };
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();

                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails
                    {
                        UserName = userEmail,
                        OrderCode = orderCode,
                        ProductId = cart.ProductId,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        ShippingAddress = model.ShippingAddress,
                        Notes = model.Notes,
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber
                    };
                    _dataContext.Add(orderDetails);
                }

                await _dataContext.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                TempData["success"] = "Đặt hàng thành công. Vui lòng chờ duyệt đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }
            else
            {
                TempData["error"] = "Vui lòng điền đầy đủ thông tin để tiến hành thanh toán.";
                return View(model);
            }
        }
        #region Paypal payment
        [Authorize]
        [HttpPost("/Checkout/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            var grandTotal = cartItems.Sum(item => item.Price * item.Quantity);

            // Gửi thông tin đơn hàng qua PayPal
            var tongTien = grandTotal.ToString();
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Checkout/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            var grandTotal = cartItems.Sum(item => item.Price * item.Quantity);

            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // Lưu thông tin đơn hàng vào database
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel
                {
                    OrderCode = orderCode,
                    UserName = userEmail,
                    Status = 1,
                    CreateDate = DateTime.Now
                };
                _dataContext.Add(orderItem);
                await _dataContext.SaveChangesAsync();

                foreach (var cart in cartItems)
                {
                    var orderDetails = new OrderDetails
                    {
                        UserName = userEmail,
                        OrderCode = orderCode,
                        ProductId = cart.ProductId,
                        Price = cart.Price,
                        Quantity = cart.Quantity,
                        // Điền các thông tin khác của đơn hàng ở đây
                    };
                    _dataContext.Add(orderDetails);
                }

                await _dataContext.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");

                TempData["success"] = "Checkout thành công. Vui lòng chờ duyệt đơn hàng!";
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion

    }
}
