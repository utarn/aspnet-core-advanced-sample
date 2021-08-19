using MediatR;

using Microsoft.AspNetCore.Identity;

using MvcDay1.Data;

using System.Threading;
using System.Threading.Tasks;

namespace MvcDay1.Applications.Member.Commands.UserLoginCommand
{
    public class UserLoginCommand : IRequest<RedirectResultModel>
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, RedirectResultModel>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public UserLoginCommandHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }
            public async Task<RedirectResultModel> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {

                var user = await _userManager.FindByNameAsync(request.UserName);
                if (await _userManager.IsInRoleAsync(user, "Administrator"))
                {
                    return new RedirectResultModel()
                    {
                        ActionName = "Index",
                        ControllerName = "Admin"
                    };
                }
                else if (await _userManager.IsInRoleAsync(user, "Moderator"))
                {
                    return new RedirectResultModel()
                    {
                        ActionName = "Index",
                        ControllerName = "Books"
                    };
                }
                else
                {
                    return new RedirectResultModel()
                    {
                        ActionName = "Index",
                        ControllerName = "Books"
                    };
                }

            }
        }
    }
}
