namespace BKAC.Services
{
    public class RpcParamsDto
    {
        public string userId { get; set; }
        public string username { get; set; }
        public string identifyNumber { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string fingerPrintImage { get; set; }
        public string faceImage { get; set; }
    }
}