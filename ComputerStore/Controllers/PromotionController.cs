using ComputerStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComputerStore.Controllers
{
    [Authorize(Roles = "Manager,Staff")]
    public class PromotionController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        public PromotionController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        // Xem danh sách khuyến mãi
        public IActionResult Index()
        {
            var promos = _context.Promotions.ToList();
            return View(promos);
        }

        // Tạo mã khuyến mãi mới
        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Promotions.Add(promotion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }
    }
}