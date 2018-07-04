
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using RequestIntercessor.Models;

namespace RequestIntercessor.Services
{
    public interface IProxy{
        Task<ProxyContent> Dispatch(string forwardUrl, HttpRequest request, HeaderDictionary additionalHeaders = null);
    }
}