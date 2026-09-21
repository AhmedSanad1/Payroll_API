using System.Net;

namespace PayRollApi.Application.Common
{
    public class SuccessResponse<T> : ApiResponse<T>
    {
        public SuccessResponse(string message, T? obj)
        {
            StatusCode = HttpStatusCode.OK;
            Message = message;
            Object = obj;
        }
        public SuccessResponse(string message)
        {
            StatusCode = HttpStatusCode.OK;
            Message = message;
        }
    }
}
