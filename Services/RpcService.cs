using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BKAC.Services
{
    public class RpcService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RpcService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> SendRpcRequestAsync(RpcRequestDto requestDto)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var baseUrl = _configuration["BaseURL"];
            var url = $"{baseUrl}/api/rpc/twoway/{requestDto.DeviceId}";

            var requestBody = new
            {
                method = requestDto.Method,
                @params = requestDto.Params,
                persistent = false,
                timeout = 5000
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestDto.Token);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }
    }


}