using FluentValidation.Results;
using JobLess.Shared.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace JobLess.Shared.Domain.Common
{
    public class ValidationExceptionThrower : IValidationExceptionThrower
    {
        public void ThrowValidationException(string name, string message)
        {
            throw new FluentValidation.ValidationException(
                new List<ValidationFailure> {
                new(name,message)
                }
            );
        }
    }

}
