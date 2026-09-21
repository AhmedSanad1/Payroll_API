using System.Net;
using FluentValidation.Results;

namespace PayRollApi.Application.Common
{
    public static class ValidationResultExtensions
    {
        public static FailResponse<T> ToFailResponse<T>(this ValidationResult validation) =>
            new(validation.Errors.Count > 0 ? validation.Errors[0].ErrorMessage : "Validation failed",
                validation.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()),
                HttpStatusCode.BadRequest);
    }
}
