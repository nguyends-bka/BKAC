using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

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

        // GET: api/History?historyId=5 (Lấy theo historyId từ query string)
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

        // GET: api/History?userIds=1&userIds=2&userIds=3&deviceIds=4&deviceIds=5&startTime=2025-04-10T08:30:00&endTime=2025-04-10T17:00:00
        [HttpGet("userIds/deviceIds")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUsersAndDevices(
            [FromQuery] List<string> userIds, 
            [FromQuery] List<string> deviceIds, 
            [FromQuery] DateTime startTime, 
            [FromQuery] DateTime endTime)
        {
            // Lọc lịch sử theo UserIds, DeviceIds và kiểm tra nếu Timestamp nằm trong khoảng thời gian
            var histories = await _context.Histories
                .Where(h => userIds.Contains(h.UserId.ToString()) &&  // Chuyển UserId thành string để so sánh với danh sách userIds
                            deviceIds.Contains(h.DeviceId.ToString()) && // Chuyển DeviceId thành string để so sánh với danh sách deviceIds
                            h.Timestamp >= startTime && 
                            h.Timestamp <= endTime)
                .ToListAsync();  // Lọc dữ liệu theo UserIds, DeviceIds và Timestamp

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

            return CreatedAtAction(nameof(GetHistoryById), new { historyId = history.HistId }, history);
        }

        // PUT: api/History?historyId=5 (Cập nhật lịch sử theo historyId từ query string)
        [HttpPut]
        [Route("historyId")]
        public async Task<IActionResult> UpdateHistory([FromQuery] int historyId, [FromBody] History history)
        {
            var existingHistory = await _context.Histories.FindAsync(historyId);
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

        // DELETE: api/History?historyId=5 (Xóa lịch sử theo historyId từ query string)
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
