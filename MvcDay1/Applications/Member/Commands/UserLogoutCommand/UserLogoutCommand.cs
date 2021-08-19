using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcDay1.Data;

namespace MvcDay1.Applications.Member.Commands.UserLogoutCommand
{
    public class UserLogoutCommand : IRequest<bool>
    {
        public class UserLogoutCommandHandler : IRequestHandler<UserLogoutCommand,bool>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;

            public UserLogoutCommandHandler(SignInManager<ApplicationUser> signInManager)
            {
                _signInManager = signInManager;
            }
            public async Task<bool> Handle(UserLogoutCommand request, CancellationToken cancellationToken)
            {
                await _signInManager.SignOutAsync();
                return true;
            }
        }
    }
}
