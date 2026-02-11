using F360.Domain.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace F360.Application.Validators
{
    public class CustomValidator<T> : AbstractValidator<T>
    {
        protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
        {
            var errors = string.Join("\n", result.Errors.Select(x => x.ErrorMessage));
            throw new BusinessException(errors);
        }
    }
}
