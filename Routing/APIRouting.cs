using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace RequestIntercessor.Routing
{
    public class APIRouting : IRouter
    {
        private readonly IRouter _defaultRouter;
        private readonly IConfiguration _configuration;
        private bool TestMode => _configuration.GetValue<bool>("Settings:LocalTest");

        public APIRouting(IRouter defaultRouter, IConfiguration configuration)
        {
            _defaultRouter = defaultRouter;
            _configuration = configuration;
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return _defaultRouter.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = "Proxy";
            routeData.Values["action"] = "Forward";
            if(TestMode)
            {
                if(context.HttpContext.Request.Headers.ContainsKey("x-ProxyMirror"))
                {
                    routeData.Values["action"] = "MirrorResponse";
                }
                else
                {
                    routeData.Values["action"] = "MirrorForward";
                }
            }
            context.RouteData = routeData;
            await _defaultRouter.RouteAsync(context);
        }
    }
}