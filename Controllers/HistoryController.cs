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

        // GET: api/History/5 (Lấy theo HistId)
        [HttpGet("{id}")]
        public async Task<ActionResult<History>> GetHistoryById(int id)
        {
            var history = await _context.Histories.FindAsync(id);  // Lấy lịch sử theo HistId từ cơ sở dữ liệu
            if (history == null)
                return NotFound();
            return Ok(history);
        }

        // GET: api/History/user/5 (Lấy theo UserId)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUserId(int userId)
        {
            var histories = await _context.Histories
                .Where(h => h.UserId == userId)
                .ToListAsync();  // Lọc lịch sử theo UserId

            if (histories == null || histories.Count == 0)
                return NotFound();
            return Ok(histories);
        }

        // GET: api/History/device/5 (Lấy theo DeviceId)
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByDeviceId(int deviceId)
        {
            var histories = await _context.Histories
                .Where(h => h.DeviceId == deviceId)
                .ToListAsync();  // Lọc lịch sử theo DeviceId

            if (histories == null || histories.Count == 0)
                return NotFound();
            return Ok(histories);
        }

        // GET: api/History/user/5/device/5 (Lấy theo UserId và DeviceId)
        // [HttpGet("user/{userId}/device/{deviceId}")]
        // public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUserAndDevice(int userId, int deviceId)
        // {
        //     var histories = await _context.Histories
        //         .Where(h => h.UserId == userId && h.DeviceId == deviceId)
        //         .ToListAsync();  // Lọc lịch sử theo UserId và DeviceId

        //     if (histories == null || histories.Count == 0)
        //         return NotFound();
        //     return Ok(histories);
        // }

        // GET: api/History/user/{listUserId}/device/{listDeviceId} (Lấy lịch sử theo danh sách UserId và DeviceId)
        [HttpPost("user/{listUserId}/device/{listDeviceId}")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUsersAndDevices(string listUserId, string listDeviceId)
        {
            // Chuyển chuỗi tham số thành danh sách các ID
            var userIds = listUserId.Split(',').Select(int.Parse).ToList();  // Tách chuỗi và chuyển thành danh sách int
            var deviceIds = listDeviceId.Split(',').Select(int.Parse).ToList();  // Tách chuỗi và chuyển thành danh sách int

            // Lọc lịch sử theo UserIds và DeviceIds
            var histories = await _context.Histories
                .Where(h => userIds.Contains(h.UserId) && deviceIds.Contains(h.DeviceId))
                .ToListAsync();  // Lấy dữ liệu theo UserIds và DeviceIds

            if (histories == null || histories.Count == 0)
                return NotFound();

            return Ok(histories);
        }

        

        // POST: api/History
        [HttpPost]
        public async Task<ActionResult<History>> CreateHistory([FromBody] History history)
        {
            _context.Histories.Add(history);  // Thêm lịch sử vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetHistoryById), new { id = history.HistId }, history);
        }

        // PUT: api/History/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHistory(int id, [FromBody] History history)
        {
            var existingHistory = await _context.Histories.FindAsync(id);  // Tìm lịch sử theo ID trong cơ sở dữ liệu
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

        // DELETE: api/History/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            var history = await _context.Histories.FindAsync(id);  // Tìm lịch sử theo ID trong cơ sở dữ liệu
            if (history == null)
                return NotFound();

            _context.Histories.Remove(history);  // Xóa lịch sử khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
