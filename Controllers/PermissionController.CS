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
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Permission>>> GetAllPermissions()
        {
            var permissions = await _context.Permissions.ToListAsync();
            return Ok(permissions);
        }

        // GET: api/Permission?permissionId=5 (Lấy quyền theo ID từ query string)
        [HttpGet]
        [Route("permissionId")]
        public async Task<ActionResult<Permission>> GetPermissionById([FromQuery] string permissionId)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);

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

            return CreatedAtAction(nameof(GetPermissionById), new { permissionId = permission.Id }, permission);
        }

        // PUT: api/Permission?permissionId=5 (Cập nhật quyền theo ID từ query string)
        [HttpPut]
        [Route("permissionId")]
        public async Task<IActionResult> UpdatePermission([FromQuery] string permissionId, [FromBody] Permission permission)
        {
            var existingPermission = await _context.Permissions.FindAsync(permissionId);
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

        // DELETE: api/Permission?permissionId=5 (Xóa quyền theo ID từ query string)
        [HttpDelete]
        [Route("permissionId")]
        public async Task<IActionResult> DeletePermission([FromQuery] string permissionId)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);  // Xóa quyền khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }
}
