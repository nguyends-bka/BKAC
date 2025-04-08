namespace BKAC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
