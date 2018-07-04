using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using RequestIntercessor.Models;

namespace RequestIntercessor.Services
{
    public class Proxy : IProxy
    {
        public async Task<ProxyContent> Dispatch(string forwardUrl, HttpRequest incomingRequest, HeaderDictionary additionalHeaders = null)
        {
            var result = new ProxyContent();
            var outbound = (HttpWebRequest)HttpWebRequest.Create(forwardUrl);
            outbound.CookieContainer = new CookieContainer();
            outbound.Method = incomingRequest.Method;
            outbound.ContentType = incomingRequest.ContentType;
            if(additionalHeaders != null)
            {
                foreach(var h in additionalHeaders){
                    outbound.Headers.Add(h.Key,h.Value.ToString());
                }
            }
            foreach(var h in incomingRequest.Headers)
            {
                if(!WebHeaderCollection.IsRestricted(h.Key))
                {
                    outbound.Headers[h.Key] = incomingRequest.Headers[h.Key];
                }
            }
            if(incomingRequest.Method != "GET"
               && incomingRequest.Method != "HEAD"
               && incomingRequest.ContentLength > 0)
            {
                var outboundBody = outbound.GetRequestStream();
                incomingRequest.Body.CopyTo(outboundBody);
                outboundBody.Close();
            }
            try
            {
                using (var response = (HttpWebResponse) await outbound.GetResponseAsync())
                {
                    var receiveStream = response.GetResponseStream();
                    using (var memoryStream = new MemoryStream()){
                        await receiveStream.CopyToAsync(memoryStream);
                        result.Content = memoryStream.ToArray();
                    }
                    result.ContentType = response.ContentType;
                    foreach(var h in response.Headers.AllKeys)
                    {
                        if(!WebHeaderCollection.IsRestricted(h))
                        {
                            result.Headers.Add(h,response.Headers[h]);
                        }
                    }
                    foreach(Cookie cookie in response.Cookies){
                        result.Cookies.Add(cookie);
                    }
                }
            }
            catch(WebException ex)
            {
                result.Success = false;
                    result.HTTPStatus = 500;
                if (ex.Status == WebExceptionStatus.ProtocolError &&
                    ex.Response != null)
                {
                    var resp = (HttpWebResponse) ex.Response;
                    result.HTTPStatus = (int)resp.StatusCode;
                }
            }
            return result;
        }        
    }
}
