using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using Microsoft.AspNetCore.Mvc;
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
            // Nếu CreatedAt không được truyền từ UI, gán giá trị mặc định (thời gian hiện tại)
            if (permission.CreatedAt == default(DateTime))
            {
                permission.CreatedAt = DateTime.UtcNow;
            }

            _context.Permissions.Add(permission);  // Thêm quyền vào DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetPermissionById), new { permissionId = permission.Id }, permission);
        }

        // POST: api/Permission/batch (Tạo quyền mới với nhiều UserId và DeviceId)
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Permission>>> CreatePermissionsBatch([FromBody] BatchPermissionRequest batchRequest)
        {
            var permissions = new List<Permission>();

            // Lặp qua danh sách UserIds và DeviceIds để tạo quyền
            foreach (var userId in batchRequest.UserIds)
            {
                foreach (var deviceId in batchRequest.DeviceIds)
                {
                    var permission = new Permission
                    {
                        UserId = userId,
                        DeviceId = deviceId,
                        Time_Start = batchRequest.TimeStart,
                        Time_End = batchRequest.TimeEnd,
                        CreatedAt = batchRequest.CreatedAt == default(DateTime) ? DateTime.UtcNow : batchRequest.CreatedAt // Nếu không có CreatedAt, dùng thời gian hiện tại
                    };

                    _context.Permissions.Add(permission);  // Thêm quyền vào DbContext
                    permissions.Add(permission);
                }
            }

            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return CreatedAtAction(nameof(GetAllPermissions), permissions);
        }

        // PUT: api/Permission?permissionId=5 (Cập nhật quyền theo ID từ query string)
        [HttpPost]
        [Route("{permissionId}")]
        public async Task<IActionResult> UpdatePermission([FromRoute] string permissionId, [FromBody] Permission permission)
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
        [Route("{permissionId}")]
        public async Task<IActionResult> DeletePermission([FromRoute] string permissionId)
        {
            var permission = await _context.Permissions.FindAsync(permissionId);
            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);  // Xóa quyền khỏi DbContext
            await _context.SaveChangesAsync();  // Lưu thay đổi vào cơ sở dữ liệu

            return NoContent();
        }
    }

    // Định nghĩa model cho yêu cầu batch
    public class BatchPermissionRequest
    {
        public List<string> UserIds { get; set; }  // Danh sách UserId
        public List<string> DeviceIds { get; set; }  // Danh sách DeviceId
        public DateTime TimeStart { get; set; }  // Thời gian bắt đầu
        public DateTime TimeEnd { get; set; }    // Thời gian kết thúc
        public DateTime CreatedAt { get; set; }  // Thời gian tạo quyền (được truyền từ UI)
    }
}
