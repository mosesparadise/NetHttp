﻿using System.Net.Http;
using System.Threading.Tasks;

namespace NetHttp
{
    public partial class NetHttpClient
    {
        public async Task<INetResponse> OptionsAsync(string url)
        {
            return await ExecuteAsync(HttpMethod.Options, url).ConfigureAwait(false);
        }
        public Task<INetResponse<TResponse>> OptionsJsonAsync<TRequest, TResponse>(string url, TRequest request)
        {
            HttpContent jsonContent = GetJsonContent(request);
            return OptionsAsync<TResponse>(url, jsonContent);
        }
        public async Task<INetResponse> OptionsJsonAsync<TRequest>(string url, TRequest request)
        {
            HttpContent jsonContent = GetJsonContent(request);
            return await OptionsAsync(url, jsonContent).ConfigureAwait(false);
        }
        public async Task<INetResponse<TResponse>> OptionsAsync<TResponse>(string url, HttpContent content)
        {
            var response = await ReadAsync<TResponse>(HttpMethod.Options, url, content).ConfigureAwait(false);
            return response;
        }
        public async Task<INetResponse> OptionsAsync(string url, HttpContent content)
        {
            return await ExecuteAsync(HttpMethod.Options, url, content).ConfigureAwait(false);
        }
    }
}
