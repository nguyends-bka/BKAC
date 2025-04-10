using System.ComponentModel.DataAnnotations;

namespace BKAC.Models
{
    public class History
    {
        [Key]  // Đánh dấu HistId là khóa chính
        public string HistId { get; set; }       // ID của lịch sử
        public string UserId { get; set; }       // ID người dùng
        public string DeviceId { get; set; }     // ID thiết bị
        public string Type { get; set; }      // Loại: vào hay ra
        public string Status { get; set; }    // Trạng thái: thành công hay thất bại
        public DateTime Timestamp { get; set; } // Thời gian thực hiện hành động

        // Constructor để khởi tạo HistId bằng GUID mới
        public History()
        {
            HistId = Guid.NewGuid().ToString(); // Khởi tạo HistId là string GUID
            Type = string.Empty;  // Khởi tạo Type mặc định
            Status = string.Empty;  // Khởi tạo Status mặc định
        }
    }
}
