using AiCatchGame.Bo.Exceptions;
using AiCatchGame.Web.Client.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace AiCatchGame.Web.Client.Services
{
    public class NetClient : INetClient
    {
        private static readonly JsonSerializerOptions _caseInsentive = new() { PropertyNameCaseInsensitive = true };
        private readonly IHttpClientFactory _httpClientFactory;

        public NetClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TOut?> GetAsync<TOut>(string url)
        {
            using HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage message = await client.GetAsync(url);
            string content = await message.Content.ReadAsStringAsync();
            if (!message.IsSuccessStatusCode)
            {
                await TreatError(url, message);
                throw new AiCatchException(ErrorCodes.Undefined);
            }

            TOut? result = await message.Content.ReadFromJsonAsync<TOut>();
            return result;
        }

        public async Task<TOut?> PostAsync<TIn, TOut>(string url, TIn body)
        {
            using HttpClient client = _httpClientFactory.CreateClient();
            HttpResponseMessage message = await client.PostAsJsonAsync<TIn>(url, body);
            string content = await message.Content.ReadAsStringAsync();
            if (!message.IsSuccessStatusCode)
            {
                await TreatError(url, message);
                throw new AiCatchException(ErrorCodes.Undefined);
            }

            TOut? result = await message.Content.ReadFromJsonAsync<TOut>();
            return result;
        }

        private async Task TreatError(string urlParameters, HttpResponseMessage response)
        {
            try
            {
                // TODO Quick and dirty solution, error management should be extended to all post or all requests
                string result = await response.Content.ReadAsStringAsync();

                ErrorContentDebug? errorContentDebug = null;
                errorContentDebug = JsonSerializer.Deserialize<ErrorContentDebug>(result, _caseInsentive);
                if (errorContentDebug != null && errorContentDebug.Code != 0)
                {
                    throw new AiCatchException(errorContentDebug);
                }

                ErrorContent? errorContent = null;
                errorContent = JsonSerializer.Deserialize<ErrorContent>(result, _caseInsentive);
                if (errorContent != null && errorContent.Code == 0)
                {
                    throw new AiCatchException(errorContent);
                }
                throw new AiCatchException(ErrorCodes.Undefined);
            }
            catch (NotSupportedException)
            {
                throw new AiCatchException(ErrorCodes.Undefined);
            }
        }
    }
}