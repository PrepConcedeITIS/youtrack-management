using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YouTrack.Management.Common
{
    public abstract class BaseClient
    {
        protected readonly HttpClient HttpClient;

        private readonly JsonSerializerOptions _jsonOptions;

        protected BaseClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
            _jsonOptions = DefaultJsonSerializerOptions();
        }

        private static JsonSerializerOptions DefaultJsonSerializerOptions()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            jsonOptions.Converters.Add(new JsonStringEnumConverter());

            return jsonOptions;
        }

        protected virtual string ApiName => GetType().Name;

        /// <summary>
        /// Сделать get запрос
        /// </summary>
        protected Task<(HttpStatusCode, string)> CallApiGet(string url) =>
            CallApi(client => client.GetAsync(url));

        /// <summary>
        /// Сделать post запрос
        /// </summary>
        protected Task<(HttpStatusCode, string)> CallApiPost(string url, HttpContent content) =>
            CallApi(client => client.PostAsync(url, content));

        /// <summary>
        /// Сделать put запрос
        /// </summary>
        protected Task<(HttpStatusCode, string)> CallApiPut(string url, HttpContent content = null) =>
            CallApi(client => client.PutAsync(url, content));

        /// <summary>
        /// Сделать patch запрос
        /// </summary>
        protected Task<(HttpStatusCode, string)> CallApiPatch(string url, HttpContent content = null) =>
            CallApi(client => client.PatchAsync(url, content));

        /// <summary>
        /// Сделать запрос к api
        /// </summary>
        protected virtual async Task<(HttpStatusCode, string)> CallApi(
            Func<HttpClient, Task<HttpResponseMessage>> callFunc)
        {
            var response = await callFunc(HttpClient);
            var result = await GetApiResultAsString(response);
            return (response.StatusCode, result);
        }

        /// <summary>
        /// Получить данные с api как строку
        /// </summary>
        protected virtual Task<string> GetApiResultAsString(HttpResponseMessage callApiResponse) =>
            GetApiResult(callApiResponse, callApiResponse.Content.ReadAsStringAsync);

        /// <summary>
        /// Получить результат с api
        /// </summary>
        protected virtual async Task<TResult> GetApiResult<TResult>(HttpResponseMessage callApiResponse,
            Func<Task<TResult>> readResult)
        {
            var result = await readResult();
            try
            {
                callApiResponse.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        /// <summary>
        /// Сформировать JSON представление объекта
        /// </summary>
        protected StringContent JsonContent(object data)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Сформировать объект из JSON представления
        /// </summary>
        protected T DeserializeResult<T>(string result) => JsonSerializer.Deserialize<T>(result, _jsonOptions);

        /// <summary>
        /// Создать URL
        /// </summary>
        protected virtual string BuildUrl(string path) => path;
    }
}