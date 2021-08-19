using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using WebAPIDay2.Data;

namespace WebAPIDay2.Applications.Auth.Commands.AuthenticateCommand
{
    public class AuthenticateCommand : IRequest<AccessToken>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AccessToken>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly AppSetting _appSetting;
            public AuthenticateCommandHandler(SignInManager<ApplicationUser> signInManager,
                UserManager<ApplicationUser> userManager,
                IOptions<AppSetting> appSetting)
            {
                _signInManager = signInManager;
                _userManager = userManager;
                _appSetting = appSetting.Value;
            }
            public async Task<AccessToken> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
            {

                var appUser = await _userManager.FindByNameAsync(request.Username);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_appSetting.Secret);

                var identityProperty = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                    new Claim(ClaimTypes.Name, appUser.UserName),
                };

                if (await _userManager.IsInRoleAsync(appUser, "Administrator"))
                {
                    identityProperty.Add(new Claim(ClaimTypes.Role,"Administrator"));
                }
                else
                {
                    identityProperty.Add(new Claim(ClaimTypes.Role, "User"));
                }

                var tokenDescription = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(identityProperty),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescription);
                return new AccessToken()
                {
                    Token = tokenHandler.WriteToken(token)
                };
            }
        }
    }
}
