namespace BKAC.Services
{
    public class RpcParamsDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string IdentifyNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FingerPrintImage { get; set; }
        public string FaceImage { get; set; }
    }
}