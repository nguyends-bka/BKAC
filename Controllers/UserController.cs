using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Khai báo DbContext

        // Constructor để khởi tạo _context
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/User/All (Lấy tất cả người dùng)
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();  // Lấy danh sách người dùng từ cơ sở dữ liệu
            return Ok(users);
        }

        // GET: api/User?userId=763436bb-d312-4faf-8983-7e2ae406aab2 (Lấy người dùng theo GUID từ query string)
        [HttpGet("userId")]
        public async Task<ActionResult<User>> GetUser([FromQuery] string userId)  // Sửa thành string
        {
            var user = await _context.Users.FindAsync(userId);  // Lấy người dùng theo GUID từ cơ sở dữ liệu
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/User (Tạo người dùng mới)
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            _context.Users.Add(user);  // Thêm người dùng vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);  // Trả về người dùng mới
        }

        // PUT: api/User?userId=763436bb-d312-4faf-8983-7e2ae406aab2 (Cập nhật người dùng theo GUID từ query string)
        [HttpPut("userId")]
        public async Task<IActionResult> UpdateUser([FromQuery] string userId, [FromBody] User user)
        {
            var existingUser = await _context.Users.FindAsync(userId);  // Tìm người dùng theo GUID trong cơ sở dữ liệu
            if (existingUser == null)
                return NotFound();

            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.FaceImg = user.FaceImg;
            existingUser.CCCD = user.CCCD;
            existingUser.Fingerprint = user.Fingerprint;

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/User?userId=763436bb-d312-4faf-8983-7e2ae406aab2 (Xóa người dùng theo GUID từ query string)
        [HttpDelete("userId")]
        public async Task<IActionResult> DeleteUser([FromQuery] string userId)  // Sửa thành string
        {
            var user = await _context.Users.FindAsync(userId);  // Tìm người dùng theo GUID trong cơ sở dữ liệu
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);  // Xóa người dùng khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
