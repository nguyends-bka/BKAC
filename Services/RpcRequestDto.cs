
using BKAC.Services;
namespace BKAC.Services
{
    public class RpcRequestDto
    {
        public required string Token { get; set; }
        public required string Method { get; set; }
        public required string DeviceId { get; set; }
        public required RpcParamsDto Params { get; set; }
    }
}