using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using RequestIntercessor.Services;
using RequestIntercessor.Models;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace RequestIntercessor.Controllers
{
    public class ProxyController : Controller
    {
        private readonly IConfiguration _configuration;
        private string ForwardUrl => _configuration.GetValue<string>("Settings:ForwardUrl");
        private readonly IProxy _proxy;
        public ProxyController(IConfiguration configuraiton
                            ,IProxy proxy){
            _configuration = configuraiton;
            _proxy = proxy;
        }

        private IActionResult Dispatch(string forwardUrl, HttpRequest incomingRequest, HeaderDictionary additionalHeaders = null)
        {
            var proxyResult = _proxy.Dispatch(forwardUrl, Request, additionalHeaders).Result;
            if(proxyResult.Success)
            {
                foreach(var h in proxyResult.Headers)
                {
                    if(h.Key != "Set-Cookie")
                    {
                        Response.Headers[h.Key] = h.Value;
                    }

                }   
                foreach(var cookie in proxyResult.Cookies)
                {

                    var option = new CookieOptions() {
                        Domain = cookie.Domain,
                        Path = cookie.Path,
                    };
                    if(cookie.Expires > DateTime.MinValue){
                        option.Expires = cookie.Expires;
                    }
                    Response.Cookies.Append(cookie.Name, cookie.Value, option);
                }
                return new ProxyResult(proxyResult.Content, proxyResult.ContentType, proxyResult.HTTPStatus);;
            }
            else
            {
                return StatusCode(proxyResult.HTTPStatus);
            }
        }

        public IActionResult Forward()
        {
            var forwardUrl = $"{ForwardUrl}{Request.Path}{Request.QueryString}";
            return Dispatch(forwardUrl, Request);
        }

        public IActionResult MirrorForward()
        {
            var forwardUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";
            var addHeaders = new HeaderDictionary();
            addHeaders.Add("x-ProxyMirror","true");
            return Dispatch(forwardUrl, Request, addHeaders);
        }

        public IActionResult MirrorResponse(MirrorModel viewModel)
        {
            if(viewModel.HTTPStatus != 200)
            {
                return StatusCode(viewModel.HTTPStatus);
            }
            foreach(var ac in viewModel.AlterCookies)
            {
                Response.Cookies.Append(ac.Key, ac.Value);
            }
            return View(viewModel);
        }
    }
}
