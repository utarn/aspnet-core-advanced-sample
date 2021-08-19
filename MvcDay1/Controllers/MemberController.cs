using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MvcDay1.Applications.Member.Commands.SocialLoginCommand;
using MvcDay1.Applications.Member.Commands.UserLoginCommand;
using MvcDay1.Applications.Member.Commands.UserLogoutCommand;
using MvcDay1.Applications.Member.Commands.UserRegisterCommand;
using MvcDay1.Applications.Member.Queries.GetSignInChallengeQuery;

namespace MvcDay1.Controllers
{
    public class MemberController : Controller
    {
        private readonly IMediator _mediator;

        public MemberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginCommand command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            var result = await _mediator.Send(command);
            return RedirectToAction(result.ActionName, result.ControllerName);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            try
            {
                await _mediator.Send(command);
            }
            catch (ValidationException e)
            {
                ModelState.AddModelError("Password", e.Message);
                return View(command);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SocialLogin([FromQuery] GetSignInChallengeQuery query)
        {
            return await _mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SocialLoginCallback([FromQuery] SocialLoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.PathName != null)
            {
                return Redirect(result.PathName);
            }
            else
            {
                return RedirectToAction(result.ActionName, result.ControllerName);
            }
        }

        public async Task<IActionResult> Logout(UserLogoutCommand command)
        {
            await _mediator.Send(command);
            return RedirectToAction("Index", "Home");
        }
    }
}
