using FluentValidation;

namespace TaskManagement.Application.Features.Authentication.ResendVerificationEmail;

public class ResendVerificationEmailValidator
    : AbstractValidator<ResendVerificationEmailCommand>
{
    public ResendVerificationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}