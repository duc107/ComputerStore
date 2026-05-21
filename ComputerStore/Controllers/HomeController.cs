using ComputerStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        // Gọi Database vào Trang chủ
        public HomeController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm từ DB đẩy ra View
            var products = _context.Products.ToList();
            return View(products);
        }
    }
}