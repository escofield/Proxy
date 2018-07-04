using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace RequestIntercessor.Models
{
    public class MirrorBinder : IModelBinder
    {
        private static string ProxySimulatedResponseKey = "x-simulated-response";
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var m = new MirrorModel();
            var request = bindingContext.ActionContext.HttpContext.Request;
            m.Headers = request.Headers;
            m.Method = request.Method;
            m.Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            //viewModel.Body = body;
            
            if(request.HasFormContentType)
            {
                if(request.Form.Keys.Count > 0)
                {
                    foreach(var k in request.Form.Keys)
                    {
                        m.Form.Add(k,request.Form[k]);
                    }
                }
            }

            if(request.Headers.ContainsKey(ProxySimulatedResponseKey)){
                
                if(int.TryParse(request.Headers[ProxySimulatedResponseKey], out var status))
                {
                    m.HTTPStatus = status;
                }
            }

            if(m.Headers.ContainsKey("x-cookie-modification"))
            {
                var cookies = m.Headers["x-cookie-modification"][0].Split(',');
                foreach(var s in cookies)
                {
                    var kvp = s.Split(':');
                    m.AlterCookies.Add(kvp[0],kvp[1]);
                }
            }

            foreach(var q in request.Query){
                m.Query.Add(q.Key,q.Value);
            }
            bindingContext.Result = ModelBindingResult.Success(m);
            return Task.CompletedTask;
        }
    }
}