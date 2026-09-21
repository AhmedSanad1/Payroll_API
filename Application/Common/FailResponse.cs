using System.Net;

namespace PayRollApi.Application.Common
{
    public class FailResponse<T> : ApiResponse<T>
    {
        public FailResponse(string message, T? obj)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Message = message;
            Object = obj;
        }
        public FailResponse(string message)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Message = message;
        }
        public FailResponse(string message, HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            Message = message;
        }
        public FailResponse(string message, IDictionary<string, string[]> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }
    }
}
