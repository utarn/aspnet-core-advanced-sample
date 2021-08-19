using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using MvcDay1.Applications.Member.Commands.UserLoginCommand;
using MvcDay1.Data;

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MvcDay1.Applications.Member.Commands.SocialLoginCommand
{
    public class SocialLoginCommand : IRequest<RedirectResultModel>
    {
        public string? ReturnUrl { get; set; }
        public string? RemoteError { get; set; }

        public class SocialLoginCommandHandler : IRequestHandler<SocialLoginCommand, RedirectResultModel>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ILogger<SocialLoginCommandHandler> _logger;

            public SocialLoginCommandHandler(
                SignInManager<ApplicationUser> signInManager,
                UserManager<ApplicationUser> userManager,
                ILoggerFactory loggerFactory)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _logger = loggerFactory.CreateLogger<SocialLoginCommandHandler>();
            }

            public async Task<RedirectResultModel> Handle(SocialLoginCommand request,
                CancellationToken cancellationToken)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return new RedirectResultModel() { ActionName = "Index", ControllerName = "Home" };
                }

                //await _context.SaveChangesAsync(cancellationToken);
                // Sign in the user with this external login provider if the user already has a login.
                var result =
                    await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                        isPersistent: false);
                if (result.Succeeded)
                {
                    // Update any authentication tokens if login succeeded
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                    _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                    if (request.ReturnUrl == null)
                    {
                        return new RedirectResultModel() { ActionName = "Index", ControllerName = "Books" };
                    }
                    else
                    {
                        return new RedirectResultModel() { PathName = request.ReturnUrl };
                    }
                }

                // If the user does not have an account, then ask the user to create an account.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                ApplicationUser? user = user = new ApplicationUser()
                {
                    Email = email,
                    UserName = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    EmailConfirmed = true,
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    identityResult = await _userManager.AddLoginAsync(user, info);
                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.",
                            info.LoginProvider);

                        // Update any authentication tokens as well
                        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                        if (request.ReturnUrl == null)
                        {
                            return new RedirectResultModel() { ActionName = "Index", ControllerName = "Books" };
                        }
                        else
                        {
                            return new RedirectResultModel() { PathName = request.ReturnUrl };
                        }
                    }
                }

                return new RedirectResultModel() { ActionName = "Index", ControllerName = "Books" };
            }
        }
    }
}
