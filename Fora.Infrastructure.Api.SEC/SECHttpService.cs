using Fora.Infrastructure.Api.SEC.Converters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace Fora.Infrastructure.Api.SEC
{
    public class SECHttpService
    {

        #region Properties
        //"CaseSchedules/ListEvent/CaseImport";

        ILogger<SECHttpService> _logger;
        IHttpClientFactory _httpClientFactory;

        #endregion

        #region Constructors

        public SECHttpService(
            IHttpClientFactory httpClientFactory,
           ILogger<SECHttpService> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        #endregion

        #region Methods

        public async Task<T> Get<T>(string absoluteUrl)
            where T : class
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, absoluteUrl);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                
                request.Headers.UserAgent.Clear();
                request.Headers.UserAgent.ParseAdd("PostmanRuntime/7.34.0");

                var res = await client.SendAsync(request);


                if (res.StatusCode != System.Net.HttpStatusCode.OK)
                    await LogAndErrorWhenNotOkResponse(absoluteUrl, res);

                var content = await res.Content.ReadAsStringAsync();
                try
                {
                    var options = new JsonSerializerOptions();
                    var obj = System.Text.Json.JsonSerializer.Deserialize<T>(content, options);

                    return obj;
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, content);
                    await LogAndErrorWhenNotOkResponse(absoluteUrl, res, $"Failed to convert data for company info body");
                }

                return null;
            }
        }

        private async Task LogAndErrorWhenNotOkResponse(string url, HttpResponseMessage res, object reqBody = null)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine($"Status: '{res.StatusCode}'");
                sb.AppendLine($"Reason: '{res.ReasonPhrase}'");
                sb.AppendLine($"Url: {url}");

                if (reqBody != null)
                {
                    var body = System.Text.Json.JsonSerializer.Serialize(reqBody, new JsonSerializerOptions() { WriteIndented = true });
                    sb.AppendLine($"Req Body: {body}");
                }

                string content = string.Empty;
                if (String.Compare(res.Content.Headers.ContentType.MediaType, "application/octet-stream", true) != 0
                    || String.Compare(res.Content.Headers.ContentType.MediaType, "application/pdf", true) != 0)
                {
                    content = await res.Content.ReadAsStringAsync();
                    sb.AppendLine($"Content: No content, content type was {res.Content.Headers.ContentType.MediaType}");
                }
                else
                {
                    content = await res.Content.ReadAsStringAsync();

                    sb.AppendLine("Content:");
                    sb.AppendLine(content);
                }
                //This is done to get a stack trace, it doesn't get a full stack trace though...
                throw new Exception(sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Status Code: {res.StatusCode}", $"Status Description: {res.ReasonPhrase}");
                //Basically throw exceptions for everything but no content
                if (res.StatusCode != System.Net.HttpStatusCode.NoContent && res.StatusCode != HttpStatusCode.NotFound)
                    throw new Exception($"Status: '{res.StatusCode}' from {url}", ex);
            }

        }

        
        #endregion

    }
}
