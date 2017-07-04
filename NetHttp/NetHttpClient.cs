﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NetHttp
{
    public partial class NetHttpClient : INetHttpClient
    {
        private readonly HttpClient _httpClient;
        public string BaseUrl { get; set; }
        private readonly IDeserializer _deserializer = new JsonDeserializer();
        public Dictionary<string, string> DefaultHeaders { get; } = new Dictionary<string, string>()
        {
            {"Accept", "application/json" },
            {"User-Agent", "NetHTTP/1.0" }
        };
        public NetHttpClient(string baseUrl)
        {
            BaseUrl = baseUrl ?? throw new ArgumentException(nameof(baseUrl));
            if (!BaseUrl.EndsWith("/"))
            {
                BaseUrl += "/";
            }
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }                 
        public async Task<IHttpResponse<TResponse>> ReadAsync<TResponse>(HttpMethod method, string url, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };
            var sendResponse = await HttpSendAsync(request).ConfigureAwait(false);
            var response = new HttpResponse<TResponse>()
            {
                Content = sendResponse.Content,
                StatusCode = sendResponse.StatusCode,
                Headers = sendResponse.Headers
            };
            //If HttpSendAsync already adds exception request has already failed.
            if(response.Exception == null)
            {
                try
                {
                    var typedData = await _deserializer.Deserialize<TResponse>(response.Content).ConfigureAwait(false);
                    response.Data = typedData;
                }
                catch (Exception exception)
                {
                    response.Exception = new DeserialisationException(exception);
                }
            }           
            
            return response;
        }
        public async Task<IHttpResponse> ExecuteAsync(HttpMethod method, string url, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };
            var sendResponse = await HttpSendAsync(request).ConfigureAwait(false);            
            return sendResponse;
        }
        private async Task<IHttpResponse> HttpSendAsync(HttpRequestMessage request)
        {
            foreach (KeyValuePair<string, string> keyValuePair in DefaultHeaders)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }
            
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            
            var typedResponse = new HttpResponse {StatusCode = response.StatusCode, Headers = response.Headers};
            try
            {
                typedResponse.Content = new StreamReader(stream).ReadToEnd();
            }
            catch (Exception exception)
            {
                typedResponse.Exception = new ContentReadException(exception);
            }
            return typedResponse;            
        }
        private StringContent GetJsonContent(object @object)
        {
            var json = JsonConvert.SerializeObject(@object);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public void Dispose()
        {
            _httpClient?.Dispose();
        }        
    }
}
