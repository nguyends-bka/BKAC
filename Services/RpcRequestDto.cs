
using BKAC.Services;
namespace BKAC.Services
{
    public class RpcRequestDto
    {
        public string Token { get; set; }
        public string Method { get; set; }
        public string DeviceId { get; set; }
        public RpcParamsDto Params { get; set; }
    }
}