using Microsoft.AspNetCore.Mvc;
using BKAC.Models;
using OfficeOpenXml;
using System.IO;

namespace BKAC.Controllers
{
    public class ImportDevicesFromExcelRequest
    {
        public IFormFile ExcelFile { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        // POST: api/Device/import-excel
        [HttpPost("import-excel")]
        public async Task<ActionResult<List<Device>>> ImportDevicesFromExcel([FromForm] ImportDevicesFromExcelRequest request)
        {
            if (request.ExcelFile == null || request.ExcelFile.Length <= 0)
                return BadRequest("File không được để trống");

            if (!Path.GetExtension(request.ExcelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("File phải có định dạng .xlsx");

            var devices = new List<Device>();

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
                        var device = new Device
                        {
                            Name = worksheet.Cells[row, 1].Value?.ToString() ?? "",
                            Type = worksheet.Cells[row, 2].Value?.ToString() ?? "",
                            Label = worksheet.Cells[row, 3].Value?.ToString() ?? ""
                        };

                        // Validate dữ liệu
                        if (string.IsNullOrWhiteSpace(device.Name) || 
                            string.IsNullOrWhiteSpace(device.Type) || 
                            string.IsNullOrWhiteSpace(device.Label))
                        {
                            continue; // Bỏ qua dòng không hợp lệ
                        }

                        devices.Add(device);
                    }
                }
            }

            return Ok(devices);
        }
    }
}
