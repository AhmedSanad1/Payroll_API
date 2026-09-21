using System;
using System.Collections.Generic;

namespace PayRollApi.Domain.Exceptions
{
    public sealed class FieldValidationError
    {
        public string Table { get; set; } = null!;
        public string Field { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public sealed class FieldConfigurationValidationException : Exception
    {
        public IReadOnlyList<FieldValidationError> Errors { get; }

        public FieldConfigurationValidationException(List<FieldValidationError> errors)
            : base("One or more fields failed configuration validation.")
        {
            Errors = errors;
        }
    }
}