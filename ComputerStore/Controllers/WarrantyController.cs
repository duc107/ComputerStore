using Microsoft.AspNetCore.Mvc;
using ComputerStore.Models;

namespace ComputerStore.Controllers
{
    public class WarrantyController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        public WarrantyController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        public IActionResult Lookup(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber)) return View();

            // Sửa WarrantyTickets thành Warrantytickets cho khớp MySQL
            var tickets = _context.Warrantytickets
                .Where(w => w.SerialNumber == serialNumber)
                .OrderByDescending(w => w.ReceivedDate)
                .ToList();

            if (!tickets.Any()) ViewBag.Message = "Không tìm thấy thông tin cho Serial này.";

            return View(tickets);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int ticketId, string newStatus)
        {
            // Sửa WarrantyTickets thành Warrantytickets
            var ticket = _context.Warrantytickets.Find(ticketId);
            if (ticket == null) return NotFound();

            ticket.Status = newStatus;
            if (newStatus == "Done")
            {
                ticket.ReturnDate = DateTime.Now;
            }

            _context.SaveChanges();
            return Ok(new { success = true, message = "Cập nhật thành công!" });
        }
    }
}