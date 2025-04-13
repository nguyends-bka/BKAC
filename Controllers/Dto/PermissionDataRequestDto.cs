namespace BKAC.Controllers.Dto
{
    public class PermissionDataRequestDto
    {
        public List<string> UserIds { get; set; }
        public List<string> DeviceIds { get; set; }
        public DateTime Time_Start { get; set; }
        public DateTime Time_End { get; set; }

        public String Token { get; set; }
    }
}