using System;
using System.ComponentModel.DataAnnotations;

namespace BKAC.Models
{
    public class Permission
    {
        [Key]
        public string Id { get; set; }  // ID của quyền

        public string UserId { get; set; }  // ID người dùng
        public string DeviceId { get; set; }  // ID thiết bị

        public DateTime Time_Start { get; set; }  // Thời gian bắt đầu quyền
        public DateTime Time_End { get; set; }    // Thời gian kết thúc quyền

        public DateTime CreatedAt { get; set; }  // Thời gian tạo quyền (thời gian tạo)

        public Permission()
        {
            Id = Guid.NewGuid().ToString(); // Khởi tạo Id là string GUID
            UserId = string.Empty; // Khởi tạo UserId
            DeviceId = string.Empty; // Khởi tạo DeviceId
        }
    }
}
