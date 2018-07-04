using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;

namespace RequestIntercessor.Models
{
    public class ProxyContent
    {
        public bool Success { get; set; } = true;
        public int HTTPStatus { get; set; } = 200;
        public byte[] Content { get; set; }
        public HeaderDictionary Headers { get; set; } = new HeaderDictionary();
        public string ContentType { get; set; }
        public List<Cookie> Cookies { get; set;} = new List<Cookie>();
    }
}