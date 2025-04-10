using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Khai báo DbContext

        // Constructor để khởi tạo _context
        public VisitorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Visitor/All (Lấy tất cả khách thăm)
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Visitor>>> GetVisitors()
        {
            var visitors = await _context.Visitors.ToListAsync();  // Lấy danh sách khách thăm từ cơ sở dữ liệu
            return Ok(visitors);
        }

        // GET: api/Visitor?visitorId=763436bb-d312-4faf-8983-7e2ae406aab2 (Lấy khách thăm theo GUID từ query string)
        [HttpGet("visitorId")]
        public async Task<ActionResult<Visitor>> GetVisitor([FromQuery] string visitorId)  // Đảm bảo visitorId là string
        {
            var visitor = await _context.Visitors.FindAsync(visitorId);  // Lấy khách thăm theo GUID từ cơ sở dữ liệu
            if (visitor == null)
                return NotFound();
            return Ok(visitor);
        }

        // POST: api/Visitor (Tạo khách thăm mới)
        [HttpPost]
        public async Task<ActionResult<Visitor>> CreateVisitor([FromBody] Visitor visitor)
        {
            _context.Visitors.Add(visitor);  // Thêm khách thăm vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetVisitor), new { visitorId = visitor.Id }, visitor);  // Trả về khách thăm mới
        }

        // PUT: api/Visitor?visitorId=763436bb-d312-4faf-8983-7e2ae406aab2 (Cập nhật khách thăm theo GUID từ query string)
        [HttpPut("visitorId")]
        public async Task<IActionResult> UpdateVisitor([FromQuery] string visitorId, [FromBody] Visitor visitor)
        {
            var existingVisitor = await _context.Visitors.FindAsync(visitorId);  // Tìm khách thăm theo GUID trong cơ sở dữ liệu
            if (existingVisitor == null)
                return NotFound();

            existingVisitor.FullName = visitor.FullName;
            existingVisitor.VisitorName = visitor.VisitorName;
            existingVisitor.FaceImg = visitor.FaceImg;
            existingVisitor.CCCD = visitor.CCCD;
            existingVisitor.Fingerprint = visitor.Fingerprint;

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/Visitor?visitorId=763436bb-d312-4faf-8983-7e2ae406aab2 (Xóa khách thăm theo GUID từ query string)
        [HttpDelete("visitorId")]
        public async Task<IActionResult> DeleteVisitor([FromQuery] string visitorId)  // Đảm bảo visitorId là string
        {
            var visitor = await _context.Visitors.FindAsync(visitorId);  // Tìm khách thăm theo GUID trong cơ sở dữ liệu
            if (visitor == null)
                return NotFound();

            _context.Visitors.Remove(visitor);  // Xóa khách thăm khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
