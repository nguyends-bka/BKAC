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
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<History>>> GetAllHistories()
        {
            var histories = await _context.Histories.ToListAsync();  
            return Ok(histories);
        }

        // GET: api/History?historyId=5 (Lấy theo historyId từ query string)
        [HttpGet("historyId")]
        public async Task<ActionResult<History>> GetHistoryById([FromQuery] string historyId)
        {
            var history = await _context.Histories.FindAsync(historyId);  
            if (history == null)
                return NotFound();
            return Ok(history);
        }

        // GET: api/History?userId=5 (Lấy theo UserId từ query string)
        [HttpGet("userId")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByUserId([FromQuery] string userId)
        {
            var histories = await _context.Histories
                .Where(h => h.UserId == userId)  // So sánh UserId kiểu string
                .ToListAsync();

            if (histories == null || histories.Count == 0)
                return NotFound();
            return Ok(histories);
        }

        // GET: api/History?deviceId=5 (Lấy theo DeviceId từ query string)
        [HttpGet("deviceId")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistoryByDeviceId([FromQuery] string deviceId)
        {
            var histories = await _context.Histories
                .Where(h => h.DeviceId == deviceId)  // So sánh DeviceId kiểu string
                .ToListAsync();

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
            var histories = await _context.Histories
                .Where(h => userIds.Contains(h.UserId) &&  // So sánh kiểu string
                            deviceIds.Contains(h.DeviceId) && // So sánh kiểu string
                            h.Timestamp >= startTime && 
                            h.Timestamp <= endTime)
                .ToListAsync();  

            if (histories == null || histories.Count == 0)
                return NotFound();

            return Ok(histories);
        }

        // POST: api/History (Tạo lịch sử mới)
        [HttpPost]
        public async Task<ActionResult<History>> CreateHistory([FromBody] History history)
        {
            _context.Histories.Add(history);  
            await _context.SaveChangesAsync();  

            return CreatedAtAction(nameof(GetHistoryById), new { historyId = history.HistId }, history);
        }

        // PUT: api/History?historyId=5 (Cập nhật lịch sử theo historyId từ query string)
        [HttpPut("historyId")]
        public async Task<IActionResult> UpdateHistory([FromQuery] string historyId, [FromBody] History history)
        {
            var existingHistory = await _context.Histories.FindAsync(historyId);
            if (existingHistory == null)
                return NotFound();

            existingHistory.UserId = history.UserId;
            existingHistory.DeviceId = history.DeviceId;
            existingHistory.Type = history.Type;
            existingHistory.Status = history.Status;
            existingHistory.Timestamp = history.Timestamp;

            await _context.SaveChangesAsync();  
            return NoContent();
        }

        // DELETE: api/History?historyId=5 (Xóa lịch sử theo historyId từ query string)
        [HttpDelete("historyId")]
        public async Task<IActionResult> DeleteHistory([FromQuery] string historyId)
        {
            var history = await _context.Histories.FindAsync(historyId);  
            if (history == null)
                return NotFound();

            _context.Histories.Remove(history);  
            await _context.SaveChangesAsync();  

            return NoContent();
        }
    }
}
