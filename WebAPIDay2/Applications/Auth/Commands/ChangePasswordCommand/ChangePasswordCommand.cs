using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPIDay2.Data;

namespace WebAPIDay2.Applications.Auth.Commands.ChangePasswordCommand
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set;}

        public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;

            public ChangePasswordCommandHandler(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
            }

            public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, false);
                if (result.Succeeded)
                {
                    var appUser = await _userManager.FindByNameAsync(request.Username);
                    var changePasswordResult =
                        await _userManager.ChangePasswordAsync(appUser, request.Password, request.NewPassword);
                    if (changePasswordResult.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        throw new ValidationException(new ValidationFailure[]
                        {
                            new ValidationFailure("NewPassword", "The new password must satisfy the password policy."),
                        });
                    }
                }
                else
                {
                    throw new ValidationException("The matched credential is not found.");
                }
            }
        }
    }
}
