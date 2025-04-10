using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Khai báo DbContext

        // Constructor để khởi tạo _context
        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/History (Lấy tất cả lịch sử)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<History>>> GetAllHistories()
        {
            var histories = await _context.Histories.ToListAsync();  // Lấy tất cả lịch sử từ cơ sở dữ liệu
            return Ok(histories);
        }

        // GET: api/History?historyId=5 (Lấy theo HistId từ query string)
        [HttpGet]
        [Route("historyId")]
        public async Task<ActionResult<History>> GetHistoryById([FromQuery] int historyId)
        {
            var history = await _context.Histories.FindAsync(historyId);  // Lấy lịch sử theo HistId từ cơ sở dữ liệu
            if (history == null)
                return NotFound();
            return Ok(history);
        }

        // GET: api/History?userId=5 (Lấy theo UserId từ query string)
        [HttpGet("userId")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUserId([FromQuery] int userId)
        {
            var histories = await _context.Histories
                .Where(h => h.UserId == userId)
                .ToListAsync();  // Lọc lịch sử theo UserId

            if (histories == null || histories.Count == 0)
                return NotFound();
            return Ok(histories);
        }

        // GET: api/History?deviceId=5 (Lấy theo DeviceId từ query string)
        [HttpGet("deviceId")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByDeviceId([FromQuery] int deviceId)
        {
            var histories = await _context.Histories
                .Where(h => h.DeviceId == deviceId)
                .ToListAsync();  // Lọc lịch sử theo DeviceId

            if (histories == null || histories.Count == 0)
                return NotFound();
            return Ok(histories);
        }

        // GET: api/History?userIds=1,2,3&deviceIds=4,5 (Lấy lịch sử theo danh sách UserId và DeviceId từ query string)
        [HttpGet("userIds/deviceIds")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUsersAndDevices([FromQuery] string userIds, [FromQuery] string deviceIds)
        {
            var userIdList = userIds.Split(',').Select(int.Parse).ToList();  // Chuyển chuỗi thành danh sách UserIds
            var deviceIdList = deviceIds.Split(',').Select(int.Parse).ToList();  // Chuyển chuỗi thành danh sách DeviceIds

            var histories = await _context.Histories
                .Where(h => userIdList.Contains(h.UserId) && deviceIdList.Contains(h.DeviceId))
                .ToListAsync();  // Lọc lịch sử theo UserIds và DeviceIds

            if (histories == null || histories.Count == 0)
                return NotFound();

            return Ok(histories);
        }

        // POST: api/History (Tạo lịch sử mới)
        [HttpPost]
        public async Task<ActionResult<History>> CreateHistory([FromBody] History history)
        {
            _context.Histories.Add(history);  // Thêm lịch sử vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetHistoryById), new { id = history.HistId }, history);
        }

        // PUT: api/History?historyId=5 (Cập nhật lịch sử theo HistId từ query string)
        [HttpPut]
        [Route("historyId")]
        public async Task<IActionResult> UpdateHistory([FromQuery] int historyId, [FromBody] History history)
        {
            var existingHistory = await _context.Histories.FindAsync(historyId);  // Tìm lịch sử theo ID trong cơ sở dữ liệu
            if (existingHistory == null)
                return NotFound();

            existingHistory.UserId = history.UserId;
            existingHistory.DeviceId = history.DeviceId;
            existingHistory.Type = history.Type;
            existingHistory.Status = history.Status;
            existingHistory.Timestamp = history.Timestamp;

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/History?historyId=5 (Xóa lịch sử theo HistId từ query string)
        [HttpDelete]
        [Route("historyId")]
        public async Task<IActionResult> DeleteHistory([FromQuery] int historyId)
        {
            var history = await _context.Histories.FindAsync(historyId);  // Tìm lịch sử theo ID trong cơ sở dữ liệu
            if (history == null)
                return NotFound();

            _context.Histories.Remove(history);  // Xóa lịch sử khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
