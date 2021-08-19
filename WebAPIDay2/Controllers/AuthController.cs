using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WebAPIDay2.Applications.Auth.Commands.AuthenticateCommand;
using WebAPIDay2.Applications.Auth.Commands.ChangePasswordCommand;

namespace WebAPIDay2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("authenticate")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(AccessToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationException), StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> Authenticate(AuthenticateCommand model)
        {
            var result = await _mediator.Send(model);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("changepassword")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationException), StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand model)
        {
            var result = await _mediator.Send(model);
            return Ok(result);
        }
    }
}
