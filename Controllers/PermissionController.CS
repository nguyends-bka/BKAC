using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
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
        public class ImportPermissionsFromExcelRequest
        {
            public IFormFile ExcelFile { get; set; }
            public string Token { get; set; }
        }

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

        // POST: api/Permission/import-excel
        [HttpPost("import-excel")]
        public async Task<ActionResult<List<Permission>>> ImportPermissionsFromExcel([FromForm] ImportPermissionsFromExcelRequest request)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length <= 0)
                return BadRequest("File không được để trống");

            if (!Path.GetExtension(request.ExcelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File phải có định dạng .xlsx");

            var createdPermissions = new List<Permission>();

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
                        try
                        {
                            var userIdsStr = worksheet.Cells[row, 1].Value?.ToString() ?? "";
                            var deviceIdsStr = worksheet.Cells[row, 2].Value?.ToString() ?? "";
                            var timeStartStr = worksheet.Cells[row, 3].Value?.ToString() ?? "";
                            var timeEndStr = worksheet.Cells[row, 4].Value?.ToString() ?? "";

                            // Kiểm tra dữ liệu bắt buộc
                            if (string.IsNullOrWhiteSpace(userIdsStr) || 
                                string.IsNullOrWhiteSpace(deviceIdsStr) ||
                                string.IsNullOrWhiteSpace(timeStartStr) ||
                                string.IsNullOrWhiteSpace(timeEndStr))
                            {
                                continue;
                            }

                            // Parse thời gian
                            if (!DateTime.TryParse(timeStartStr, out DateTime timeStart) ||
                                !DateTime.TryParse(timeEndStr, out DateTime timeEnd))
                            {
                                continue;
                            }

                            var userIds = userIdsStr.Split(';', StringSplitOptions.RemoveEmptyEntries);
                            var deviceIds = deviceIdsStr.Split(';', StringSplitOptions.RemoveEmptyEntries);

                            foreach (var userId in userIds)
                            {
                                var user = await _context.Users.FindAsync(userId.Trim());
                                if (user == null) continue;

                                foreach (var deviceId in deviceIds)
                                {
                                    var permission = new Permission
                                    {
                                        UserId = userId.Trim(),
                                        DeviceId = deviceId.Trim(),
                                        Time_Start = timeStart,
                                        Time_End = timeEnd,
                                        CreatedAt = DateTime.UtcNow
                                    };

                                    var rpcParamsDto = new RpcParamsDto
                                    {
                                        username = user.UserName,
                                        endTime = timeEnd,
                                        startTime = timeStart,
                                        faceImage = user.FaceImg,
                                        fingerPrintImage = user.Fingerprint,
                                        identifyNumber = user.CCCD,
                                        userId = user.Id
                                    };

                                    var rpcRequestDto = new RpcRequestDto
                                    {
                                        Token = request.Token,
                                        DeviceId = deviceId.Trim(),
                                        Method = "userSchedule",
                                        Params = rpcParamsDto
                                    };

                                    await _rpcService.SendRpcRequestAsync(rpcRequestDto);

                                    _context.Permissions.Add(permission);
                                    createdPermissions.Add(permission);
                                }
                            }
                        }
                        catch
                        {
                            // Bỏ qua dòng lỗi
                            continue;
                        }
                    }
                }
            }

            if (createdPermissions.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                Message = "Permissions imported successfully",
                Count = createdPermissions.Count,
                Permissions = createdPermissions
            });
        }

        // POST: api/Permission (Tạo quyền mới)
        [HttpPost]
        public async Task<ActionResult> CreatePermission([FromBody] PermissionDataRequestDto permissions)
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
                        Time_Start = permissions.Time_Start,
                        Time_End = permissions.Time_End,
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
                    
                    rpcRequestDto.Token = permissions.Token;
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
