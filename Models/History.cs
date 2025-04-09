using System.ComponentModel.DataAnnotations;

namespace BKAC.Models
{
    public class History
    {
        [Key]  // Đánh dấu HistId là khóa chính
        public int HistId { get; set; }       // ID của lịch sử
        public int UserId { get; set; }       // ID người dùng
        public int DeviceId { get; set; }     // ID thiết bị
        public string Type { get; set; }      // Loại: vào hay ra
        public string Status { get; set; }    // Trạng thái: thành công hay thất bại
        public DateTime Timestamp { get; set; } // Thời gian thực hiện hành động
    }
}
