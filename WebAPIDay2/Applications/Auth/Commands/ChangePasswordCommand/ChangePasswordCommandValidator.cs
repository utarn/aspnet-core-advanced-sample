using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace StarmoneyKyc.Application.Auth.Commands.ChangePasswordCommand
{
    public class ChangePasswordCommandValidator : AbstractValidator<WebAPIDay2.Applications.Auth.Commands.ChangePasswordCommand.ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
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
            RuleFor(c => c.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is empty.")
                .MinimumLength(8)
                .WithMessage("Password must have minimum length of 8.")
                .NotEqual(c => c.Password)
                .WithMessage("The new password must be different from the old one.");
        }
    }
}
