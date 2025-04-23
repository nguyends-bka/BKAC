using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO;

namespace BKAC.Controllers
{
    public class ImportUsersFromExcelRequest
    {
        public IFormFile ExcelFile { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Khai báo DbContext

        static UserController()
        {
            // Đặt license context cho EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

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
        public async Task<ActionResult<User>> GetUser([FromQuery] string userId)  // Đảm bảo userId là string
        {
            var user = await _context.Users.FindAsync(userId);  // Lấy người dùng theo GUID từ cơ sở dữ liệu
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/User/import-excel
        [HttpPost("import-excel")]
        public async Task<ActionResult<List<User>>> ImportUsersFromExcel([FromForm] ImportUsersFromExcelRequest request)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length <= 0)
                return BadRequest("File không được để trống");

            if (!Path.GetExtension(request.ExcelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File phải có định dạng .xlsx");

            var users = new List<User>();

            using (var stream = new MemoryStream())
            {
                await request.ExcelFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    // Bỏ qua dòng header
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var user = new User
                        {
                            UserName = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            FullName = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                            CCCD = worksheet.Cells[row, 3].Value?.ToString() ?? "",
                            FaceImg = worksheet.Cells[row, 4].Value?.ToString() ?? "",
                            Fingerprint = worksheet.Cells[row, 5].Value?.ToString() ?? ""
                        };

                        // Validate dữ liệu
                        if (string.IsNullOrWhiteSpace(user.UserName) || 
                            string.IsNullOrWhiteSpace(user.FullName) || 
                            string.IsNullOrWhiteSpace(user.CCCD))
                        {
                            continue; // Bỏ qua dòng không hợp lệ
                        }

                        users.Add(user);
                    }
                }
            }

            if (users.Count > 0)
            {
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }

            return Ok(users);
        }

        // POST: api/User (Tạo người dùng mới)
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            _context.Users.Add(user);  // Thêm người dùng vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetUser), new { userId = user.Id }, user);  // Trả về người dùng mới
        }

        // PUT: api/User/763436bb-d312-4faf-8983-7e2ae406aab2 (Cập nhật người dùng theo GUID trong route)
        [HttpPost("{userId}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string userId, [FromBody] User user)
        {
            var existingUser = await _context.Users.FindAsync(userId);  // Tìm người dùng theo GUID trong cơ sở dữ liệu
            if (existingUser == null)
                return NotFound();

            existingUser.Id = userId;
            existingUser.FullName = user.FullName;
            existingUser.UserName = user.UserName;
            existingUser.FaceImg = user.FaceImg;
            existingUser.CCCD = user.CCCD;
            existingUser.Fingerprint = user.Fingerprint;

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/User?userId=763436bb-d312-4faf-8983-7e2ae406aab2 (Xóa người dùng theo GUID từ query string)
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userId)  // Đảm bảo userId là string
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
