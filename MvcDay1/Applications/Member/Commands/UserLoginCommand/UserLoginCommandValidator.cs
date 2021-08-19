using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using MvcDay1.Data;

namespace MvcDay1.Applications.Member.Commands.UserLoginCommand
{
    public class UserLoginCommandValidator : AbstractValidator<UserLoginCommand>
    {

        public UserLoginCommandValidator(SignInManager<ApplicationUser> signInManager)
        {
            RuleFor(c => c.UserName)
                .NotEmpty()
                .WithMessage("รหัสผู้ใช้ห้ามว่าง");

            RuleFor(c => c.Password)
                .NotEmpty()
                .WithMessage("รหัสผ่านห้ามว่าง");

            RuleFor(c => c)
                .CustomAsync(async (command, validatorContext, token) =>
                {
                    if (command.UserName != null && command.Password != null)
                    {
                        var result = await signInManager.PasswordSignInAsync(command.UserName, command.Password, false, false);

                        if (!result.Succeeded)
                        {
                            validatorContext.AddFailure(new ValidationFailure("Password", "รหัสผ่านไม่ถูกต้อง"));
                        }
                    }

                });
        }
    }
}