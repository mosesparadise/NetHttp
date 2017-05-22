﻿using System.Net.Http;
using System.Threading.Tasks;

namespace NetHttp
{
    public partial class NetHttpClient
    {
        public async Task<IHttpResponse> PutAsync(string url)
        {
            return await ExecuteAsync(HttpMethod.Put, url).ConfigureAwait(false);
        }
        public Task<IHttpResponse<TResponse>> PutJsonAsync<TRequest, TResponse>(string url, TRequest request)
        {
            HttpContent jsonContent = GetJsonContent(request);
            return PutAsync<TResponse>(url, jsonContent);
        }
        public async Task<IHttpResponse> PutJsonAsync<TRequest>(string url, TRequest request)
        {
            HttpContent jsonContent = GetJsonContent(request);
            return await PutAsync(url, jsonContent).ConfigureAwait(false);
        }
        public async Task<IHttpResponse<TResponse>> PutAsync<TResponse>(string url, HttpContent content)
        {
            var response = await ReadAsync<TResponse>(HttpMethod.Put, url, content).ConfigureAwait(false);
            return response;
        }
        public async Task<IHttpResponse> PutAsync(string url, HttpContent content)
        {
            return await ExecuteAsync(HttpMethod.Put, url, content).ConfigureAwait(false);
        }
    }
}
