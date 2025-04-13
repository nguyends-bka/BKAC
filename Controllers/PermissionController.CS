using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BKAC.Data;  // Đảm bảo bạn có DbContext ở đây
using BKAC.Models;
using BKAC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BKAC.Controllers.Dto;

namespace BKAC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
         private readonly RpcService _rpcService;
        // Constructor để khởi tạo _context
        public PermissionController(ApplicationDbContext context, RpcService rpcServic)
        {
            _context = context;
            _rpcService = rpcServic;
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
        public async Task<ActionResult> CreatePermission([FromBody] PermissionDataRequestDto permissions, String token)
        {
            var createdPermissions = new List<Permission>();

            foreach (var userId in permissions.UserIds)
            {
                foreach (var deviceId in permissions.DeviceIds)
                {
                    var permission = new Permission
                    {
                        UserId = userId,
                        DeviceId = deviceId,
                        CreatedAt = DateTime.UtcNow // Always set current time
                    };

                    var rpcParamsDto = new RpcParamsDto();
                    var rpcRequestDto = new RpcRequestDto();
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

                    rpcParamsDto.username = user.UserName;
                    rpcParamsDto.endTime = permissions.Time_End;
                    rpcParamsDto.startTime = permission.Time_Start;
                    rpcParamsDto.faceImage = user.FaceImg;
                    rpcParamsDto.fingerPrintImage = user.Fingerprint;
                    rpcParamsDto.identifyNumber = user.CCCD;
                    rpcParamsDto.userId = user.Id;
                    
                    rpcRequestDto.Token = token;
                    rpcRequestDto.DeviceId = deviceId;
                    rpcRequestDto.Method = "userSchedule";
                    rpcRequestDto.Params = rpcParamsDto;

                    var response = await _rpcService.SendRpcRequestAsync(rpcRequestDto);

                    _context.Permissions.Add(permission);
                    createdPermissions.Add(permission);
                }
            }

            await _context.SaveChangesAsync(); // Save all at once after loop

            return Ok(new
            {
                Message = "Permissions created successfully",
                Count = createdPermissions.Count,
                Permissions = createdPermissions
            });
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
}
