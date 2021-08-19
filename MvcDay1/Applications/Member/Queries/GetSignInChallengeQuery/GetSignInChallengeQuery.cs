using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MvcDay1.Data;

namespace MvcDay1.Applications.Member.Queries.GetSignInChallengeQuery
{
    public class GetSignInChallengeQuery : IRequest<ChallengeResult>
    {
        public string Provider { get; set; } = "Google";
        public string ReturnUrl { get; set; } = "/Member";

        public class GetSignInChallengeQueryHandler : IRequestHandler<GetSignInChallengeQuery, ChallengeResult>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly LinkGenerator _linkGenerator;
            private readonly SignInManager<ApplicationUser> _signInManager;
            public GetSignInChallengeQueryHandler(
                IHttpContextAccessor httpContextAccessor,
                LinkGenerator linkGenerator,    
                SignInManager<ApplicationUser> signInManager)
            {
                _httpContextAccessor = httpContextAccessor;
                _linkGenerator = linkGenerator;
                _signInManager = signInManager;
            }

            public Task<ChallengeResult> Handle(GetSignInChallengeQuery request, CancellationToken cancellationToken)
            {
                var redirectUrl = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, "SocialLoginCallback", "Member", new { request.ReturnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);
                return Task.FromResult(new ChallengeResult(request.Provider, properties));
            }
        }
    }
}
