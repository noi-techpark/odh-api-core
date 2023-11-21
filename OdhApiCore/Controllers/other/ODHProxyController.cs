// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AspNetCore.Proxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ODHProxyController : ControllerBase
    {
        
        [ApiExplorerSettings(IgnoreApi = true)]                
        [HttpGet,HttpHead, Route("v1/ODHProxy/{*url}")]        
        public Task GetODHProxy(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value + "&";
                }                

                var fullurl = url + parameter.Remove(parameter.Length - 1, 1);

                //Quick production fix
                //fullurl = fullurl.Replace("https:/", "https://");

                Console.WriteLine("Url to proxy: " + fullurl);

                return this.HttpProxyAsync(fullurl);
            }
            catch(Exception ex)
            {                
                return Task.FromException(ex);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpHead, Route("v1/HeadRequest/{*url}")]
        public async Task<IActionResult> GetGetHeadRequest(string url)
        {
            try
            {
                var parameter = "?";

                foreach (var paramdict in HttpContext.Request.Query)
                {
                    parameter = parameter + paramdict.Key + "=" + paramdict.Value + "&";
                }

                var fullurl = url + parameter.Remove(parameter.Length - 1, 1);
            
                var request = HttpContext.CreateProxyHttpRequest(new Uri(fullurl));

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                    await HttpContext.CopyProxyHttpResponse(response);

                    return new EmptyResult();
                }                

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }

    public static class ODHRequestExtensions
    {
        public static HttpRequestMessage CreateProxyHttpRequest(this HttpContext context, Uri uri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }

        public static async Task CopyProxyHttpResponse(this HttpContext context, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var response = context.Response;

            response.StatusCode = (int)responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            using (var responseStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                await responseStream.CopyToAsync(response.Body);
            }
        }
    }
}
