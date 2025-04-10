using Microsoft.AspNetCore.Mvc;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.EntityFrameworkCore;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor để khởi tạo _context
        public PermissionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Permission (Lấy tất cả quyền)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetAllPermissions()
        {
            var permissions = await _context.Permissions.ToListAsync();
            return Ok(permissions);
        }

        // GET: api/Permission/5 (Lấy quyền theo ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetPermissionById(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        // POST: api/Permission (Tạo quyền mới)
        [HttpPost]
        public async Task<ActionResult<Permission>> CreatePermission([FromBody] Permission permission)
        {
            // Đặt thời gian tạo quyền là thời gian hiện tại (UTC)
            permission.CreatedAt = DateTime.UtcNow;

            _context.Permissions.Add(permission);  // Thêm quyền vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Id }, permission);
        }

        // PUT: api/Permission/5 (Cập nhật quyền)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] Permission permission)
        {
            var existingPermission = await _context.Permissions.FindAsync(id);
            if (existingPermission == null)
                return NotFound();

            existingPermission.UserId = permission.UserId;
            existingPermission.DeviceId = permission.DeviceId;
            existingPermission.Time_Start = permission.Time_Start;
            existingPermission.Time_End = permission.Time_End;
            existingPermission.CreatedAt = permission.CreatedAt;

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu
            return NoContent();
        }

        // DELETE: api/Permission/5 (Xóa quyền theo ID)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);  // Xóa quyền khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
