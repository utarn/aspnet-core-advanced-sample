using FluentValidation;
using FluentValidation.Results;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WebAPIDay2.Data;

namespace WebAPIDay2.Applications.Auth.Commands.AuthenticateCommand
{
    public class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
    {

        public AuthenticateCommandValidator(SignInManager<ApplicationUser> signInManager)
        {
            RuleFor(c => c.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Username is empty.");
            RuleFor(c => c.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is empty.")
                .MinimumLength(8)
                .WithMessage("Password must have minimum length of 8.");

            RuleFor(c => c)
                .CustomAsync(async (command, validatorContext, token) =>
                {
                    if (command.Username != null && command.Password != null)
                    {
                        var result = await signInManager.PasswordSignInAsync(command.Username, command.Password, false, false);

                        if (!result.Succeeded)
                        {
                            validatorContext.AddFailure(new ValidationFailure("Password", "Incorrect password"));
                        }
                    }
                });
        }
    }
}
