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

        // GET: api/User (Lấy tất cả người dùng)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();  // Lấy danh sách người dùng từ cơ sở dữ liệu
            return Ok(users);
        }

        // GET: api/User?userId=5 (Lấy theo UserId từ query string)
        [HttpGet("userId")]
        public async Task<ActionResult<User>> GetUser([FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);  // Lấy người dùng theo ID từ cơ sở dữ liệu
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

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User?userId=5 (Cập nhật người dùng theo UserId từ query string)
        [HttpPut("userId")]
        public async Task<IActionResult> UpdateUser([FromQuery] int userId, [FromBody] User user)
        {
            var existingUser = await _context.Users.FindAsync(userId);  // Tìm người dùng theo ID trong cơ sở dữ liệu
            if (existingUser == null)
                return NotFound();

            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.FaceImg = user.FaceImg;  // Cập nhật FaceImg
            existingUser.CCCD = user.CCCD;  // Cập nhật CCCD
            existingUser.Fingerprint = user.Fingerprint;  // Cập nhật ảnh vân tay

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/User?userId=5 (Xóa người dùng theo UserId từ query string)
        [HttpDelete("userId")]
        public async Task<IActionResult> DeleteUser([FromQuery] int userId)
        {
            var user = await _context.Users.FindAsync(userId);  // Tìm người dùng theo ID trong cơ sở dữ liệu
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);  // Xóa người dùng khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
