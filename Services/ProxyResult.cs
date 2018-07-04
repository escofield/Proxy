using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RequestIntercessor.Services
{

    public class ProxyResult : ActionResult{
        private readonly int _statusCode = 200;
        private readonly byte[] _value;
        private readonly string _contentType;

        public ProxyResult(byte[] value)
        {
            this._value = value;
        }

        public ProxyResult(byte[] value, int statusCode) : this(value)
        {
            this._statusCode = statusCode;
        }

        public ProxyResult(byte[] value, string contentType, int statusCode = 200) : this(value, statusCode)
        {
            this._contentType = contentType;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = this._contentType;
            response.StatusCode = _statusCode;
            context.HttpContext.Response.Body.WriteAsync(_value,0,_value.Length);
            return Task.CompletedTask;
        }        
    }
}
