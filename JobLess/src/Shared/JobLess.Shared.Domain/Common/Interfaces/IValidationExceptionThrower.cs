using System;


namespace JobLess.Shared.Domain.Common.Interfaces;

public interface IValidationExceptionThrower
{
    void ThrowValidationException(string name, string message);
}
