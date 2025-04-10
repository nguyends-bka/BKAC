namespace BKAC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public string CCCD { get; set; } = string.Empty;  // Số CCCD
        public string Fingerprint { get; set; } = string.Empty;  // Ảnh vân tay (Có thể lưu dưới dạng đường dẫn file hoặc base64)



    }
}
