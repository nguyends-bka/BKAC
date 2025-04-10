using System;
using System.ComponentModel.DataAnnotations;

namespace BKAC.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }  // ID của quyền

        public int UserId { get; set; }  // ID người dùng
        public int DeviceId { get; set; }  // ID thiết bị

        public DateTime Time_Start { get; set; }  // Thời gian bắt đầu quyền
        public DateTime Time_End { get; set; }    // Thời gian kết thúc quyền

        public DateTime CreatedAt { get; set; }  // Thời gian tạo quyền (thời gian tạo)
    }
}
