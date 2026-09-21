using System.Net;

namespace PayRollApi.Application.Common
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T? Object { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
