using System;
using System.Collections.Generic;
using System.Text;
using DevUp.Common.Extensions;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(string error)
            : this(new[] { error })
        {
        }

        public ValidationException(IEnumerable<string> errors)
            : base(GetMessage(errors))
        {
            Errors = errors;
        }

        private static string GetMessage(IEnumerable<string> errors)
        {
            var builder = new StringBuilder("Validation failed");
            errors.ForEach(error => builder.AppendLine(error));
            return builder.ToString();
        }
    }
}
