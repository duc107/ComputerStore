    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ComputerStore.Models;
    using ComputerStore.Extensions;

    namespace ComputerStore.Controllers
    {
        public class CartController : Controller
        {
            private readonly ComputerStoreDbContext _context;

            public CartController(ComputerStoreDbContext context)
            {
                _context = context;
            }

            // GET: /Cart (hiển thị giỏ hàng)
            public IActionResult Index(string promoCode = "")
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            decimal subTotal = cart.Where(c => c.IsSelected).Sum(c => c.Price * c.Quantity);
            decimal discount = 0;
                int? promotionId = null;

                if (!string.IsNullOrEmpty(promoCode))
                {
                    var promo = _context.Promotions
                        .FirstOrDefault(p => p.Code == promoCode && p.ExpiryDate >= DateTime.Now);
                    if (promo != null)
                    {
                        discount = subTotal * (promo.DiscountPercent / 100);
                        promotionId = promo.PromotionId;
                        ViewBag.PromoMessage = "Áp dụng mã thành công!";
                    }
                    else
                    {
                        ViewBag.PromoError = "Mã không hợp lệ hoặc đã hết hạn!";
                    }
                }

                ViewBag.SubTotal = subTotal;
                ViewBag.Discount = discount;
                ViewBag.TotalAmount = subTotal - discount;
                ViewBag.PromoCode = promoCode;
                ViewBag.PromotionId = promotionId;

                return View(cart);
            }

        // Tích hoặc bỏ tích sản phẩm
        public IActionResult ToggleSelect(int productId)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);
                if (item != null)
                {
                    item.IsSelected = !item.IsSelected; // Đảo ngược trạng thái
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        //// POST: /Cart/AddToCart
        //[HttpPost]
        //public IActionResult AddToCart(int productId, int quantity = 1)
        //{
        //    var product = _context.Products.Find(productId);
        //    if (product == null) return NotFound();

        //    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        //    var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        cart.Add(new CartItem
        //        {
        //            ProductId = product.ProductId,
        //            ProductName = product.ProductName,
        //            ImageUrl = product.ImageUrl, // Lưu đường dẫn ảnh vào giỏ
        //            Price = product.Price,
        //            Quantity = quantity
        //        });
        //    }

        //    HttpContext.Session.Set("Cart", cart);
        //    return RedirectToAction("Index");
        //}

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            // 1. KIỂM TRA SẢN PHẨM CÒN HÀNG KHÔNG
            if (product.StockQuantity <= 0)
            {
                TempData["ErrorMsg"] = $"Sản phẩm '{product.ProductName}' đã hết hàng!";
                return RedirectToAction("Index", "Home");
            }

            var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            // 2. KIỂM TRA NẾU THÊM VÀO GIỎ CÓ VƯỢT QUÁ TỒN KHO KHÔNG
            int totalRequested = (existingItem != null ? existingItem.Quantity : 0) + quantity;
            if (totalRequested > product.StockQuantity)
            {
                TempData["ErrorMsg"] = $"Chỉ còn {product.StockQuantity} sản phẩm '{product.ProductName}' trong kho. Bạn không thể mua thêm!";
                return RedirectToAction("Index", "Home"); // Hoặc trả về giỏ hàng tùy bạn
            }

            // Nếu an toàn thì thêm vào giỏ
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            HttpContext.Session.Set("Cart", cart);
            return RedirectToAction("Index"); // Chuyển sang giỏ hàng
        }

        // HÀM MỚI 1: Tăng/Giảm số lượng
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);
                if (item != null)
                {
                    // Nếu bấm nút giảm số lượng về 0 thì xóa luôn (không cần check kho)
                    if (quantity <= 0)
                    {
                        cart.Remove(item);
                        HttpContext.Session.Set("Cart", cart);
                        return RedirectToAction("Index");
                    }

                    // TÌM SẢN PHẨM TRONG DATABASE ĐỂ CHECK KHO
                    var product = _context.Products.Find(productId);
                    if (product != null)
                    {
                        // NẾU SỐ LƯỢNG YÊU CẦU LỚN HƠN TỒN KHO -> BÁO LỖI VÀ CHẶN LẠI
                        if (quantity > product.StockQuantity)
                        {
                            TempData["ErrorMsg"] = $"Rất tiếc! Chỉ còn {product.StockQuantity} sản phẩm '{product.ProductName}' trong kho. Bạn không thể tăng thêm!";
                            // Không cập nhật số lượng, giữ nguyên số cũ
                        }
                        else
                        {
                            // Nếu hợp lệ thì cho phép cập nhật
                            item.Quantity = quantity;
                        }

                        // Lưu lại giỏ hàng
                        HttpContext.Session.Set("Cart", cart);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // HÀM MỚI 2: Xóa 1 sản phẩm
        public IActionResult RemoveItem(int productId)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            if (cart != null)
            {
                var item = cart.FirstOrDefault(c => c.ProductId == productId);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.Set("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        // HÀM MỚI 3: Xóa sạch giỏ hàng
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }
        //// POST: /Cart/AddToCart
        //[HttpPost]
        //public IActionResult AddToCart(int productId, int quantity = 1)
        //{
        //    var product = _context.Products.Find(productId);
        //    if (product == null) return NotFound();

        //    var cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        //    var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        cart.Add(new CartItem
        //        {
        //            ProductId = product.ProductId,
        //            ProductName = product.ProductName,
        //            Price = product.Price,
        //            Quantity = quantity
        //        });
        //    }

        //    HttpContext.Session.Set("Cart", cart);
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> Checkout(decimal totalAmount, int? promotionId)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");

            // CHỈ LẤY NHỮNG MÓN ĐƯỢC TÍCH CHỌN
            var selectedItems = cart?.Where(c => c.IsSelected).ToList();

            if (selectedItems == null || !selectedItems.Any())
            {
                TempData["ErrorMsg"] = "Vui lòng tích chọn ít nhất 1 sản phẩm để thanh toán!";
                return RedirectToAction("Index");
            }

            var user = await _context.Appusers.FindAsync(1);
            if (user == null)
            {
                user = new Appuser { FullName = "Khách hàng mặc định", Email = "noemail@example.com", PasswordHash = "temphash", Role = "Member" };
                _context.Appusers.Add(user);
                await _context.SaveChangesAsync();
            }

            var order = new Order { UserId = user.UserId, TotalAmount = totalAmount, Status = "Pending", OrderDate = DateTime.Now, PromotionId = promotionId };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // DÙNG selectedItems THAY VÌ cart
            foreach (var item in selectedItems)
            {
                _context.Orderdetails.Add(new Orderdetail { OrderId = order.OrderId, ProductId = item.ProductId, Quantity = item.Quantity, UnitPrice = item.Price });

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null && product.StockQuantity >= item.Quantity)
                {
                    product.StockQuantity -= item.Quantity;
                }
            }
            await _context.SaveChangesAsync();

            // QUAN TRỌNG: CHỈ XÓA NHỮNG MÓN ĐÃ MUA, GIỮ LẠI MÓN CHƯA MUA
            cart.RemoveAll(c => c.IsSelected);
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        }

        // POST: /Cart/Checkout

        //[HttpPost]
        //public async Task<IActionResult> Checkout(decimal totalAmount, int? promotionId)
        //{
        //    var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
        //    if (cart == null || !cart.Any()) return RedirectToAction("Index");

        //    // Đảm bảo có user với UserId = 1 (nếu chưa có thì tự tạo)
        //    var user = await _context.Appusers.FindAsync(1);
        //    if (user == null)
        //    {
        //        user = new Appuser
        //        {
        //            FullName = "Khách hàng mặc định",
        //            Email = "noemail@example.com",
        //            PasswordHash = "temphash",
        //            Role = "Member"
        //        };
        //        _context.Appusers.Add(user);
        //        await _context.SaveChangesAsync();
        //    }

        //    // Tạo đơn hàng
        //    var order = new Order
        //    {
        //        UserId = user.UserId,
        //        TotalAmount = totalAmount,
        //        Status = "Pending",
        //        OrderDate = DateTime.Now,
        //        PromotionId = promotionId
        //    };
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    // Thêm chi tiết đơn hàng & trừ kho
        //    foreach (var item in cart)
        //    {
        //        _context.Orderdetails.Add(new Orderdetail
        //        {
        //            OrderId = order.OrderId,
        //            ProductId = item.ProductId,
        //            Quantity = item.Quantity,
        //            UnitPrice = item.Price
        //        });

        //        var product = await _context.Products.FindAsync(item.ProductId);
        //        if (product != null && product.StockQuantity >= item.Quantity)
        //        {
        //            product.StockQuantity -= item.Quantity;
        //        }
        //    }
        //    await _context.SaveChangesAsync();

        //    // Xóa giỏ hàng
        //    HttpContext.Session.Remove("Cart");

        //    // CHỖ NÀY ĐÃ ĐƯỢC SỬA LẠI ĐỂ GỌI ĐÚNG FILE OrderSuccess CỦA BẠN
        //    return RedirectToAction("OrderSuccess", new { orderId = order.OrderId });
        //}

        // ĐÃ XÓA CÁI HÀM OrderCompleted BỊ LỖI ĐI, CHỈ GIỮ LẠI OrderSuccess
        public IActionResult OrderSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        // GET: /Cart/History (Xem danh sách đơn hàng đã mua)
        public IActionResult History()
        {
            int currentUserId = 1; // Khách hàng mặc định lúc nãy ta tạo

            // Lấy các đơn hàng của user này, sắp xếp mới nhất lên đầu
            var orders = _context.Orders
                .Where(o => o.UserId == currentUserId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: /Cart/OrderDetails/5 (Xem chi tiết 1 đơn hàng)
        public IActionResult OrderDetails(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null) return NotFound();

            // Lấy chi tiết đơn hàng (Join với bảng Product để lấy tên sản phẩm)
            var details = (from od in _context.Orderdetails
                           join p in _context.Products on od.ProductId equals p.ProductId
                           where od.OrderId == id
                           select new
                           {
                               ProductName = p.ProductName,
                               ImageUrl = p.ImageUrl,
                               Quantity = od.Quantity,
                               UnitPrice = od.UnitPrice,
                               Total = od.Quantity * od.UnitPrice
                           }).ToList();

            ViewBag.OrderInfo = order;
            ViewBag.OrderDetails = details;

            return View();
        }
        // GET: /Cart/PrintInvoice/5
        public IActionResult PrintInvoice(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null) return NotFound();

            var details = (from od in _context.Orderdetails
                           join p in _context.Products on od.ProductId equals p.ProductId
                           where od.OrderId == id
                           select new
                           {
                               ProductName = p.ProductName,
                               Quantity = od.Quantity,
                               UnitPrice = od.UnitPrice,
                               Total = od.Quantity * od.UnitPrice
                           }).ToList();

            ViewBag.OrderInfo = order;
            ViewBag.OrderDetails = details;

            // Khách hàng
            var user = _context.Appusers.Find(order.UserId);
            ViewBag.CustomerName = user?.FullName ?? "Khách Mua Lẻ";

            return View(); // Gọi đến trang giao diện in
        }
    }
}