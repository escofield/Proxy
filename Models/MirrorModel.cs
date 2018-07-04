using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
namespace RequestIntercessor.Models
{
    public class MirrorModel
    {
        public IHeaderDictionary Headers { get; set;}
        public string Body { get; set; } = "";
        public string Url { get; set; } = "";
        public IDictionary<string,string> Form { get; set; } = new Dictionary<string,string>();
        public int HTTPStatus { get; set; } = 200;
        public Dictionary<string,string> AlterCookies { get; set; } = new Dictionary<string,string>();
        public string Method { get; set; }
        public Dictionary<string,string> Query { get; set; } = new Dictionary<string,string>();
    }
}