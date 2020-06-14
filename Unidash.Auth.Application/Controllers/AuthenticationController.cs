﻿using AspNet.Security.OAuth.Discord;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Unidash.Auth.Users.Commands;
using Unidash.Auth.Users.Requests;

namespace Unidash.Auth.Application.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public static Dictionary<string, string> ProviderMapping => new Dictionary<string, string>
        {
            { "discord", DiscordAuthenticationDefaults.AuthenticationScheme }
        };

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            return await _mediator.Send(request);
        }

        [HttpGet("connect/challenge/{provider}")]
        [AllowAnonymous]
        public async Task Challenge(string provider)
        {
            // TODO: Challenge command
            if (!ProviderMapping.ContainsKey(provider))
                throw new Exception("Provider does not exist");

            await HttpContext.ChallengeAsync(ProviderMapping[provider], new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Finalize))
            });
        }

        [HttpGet("connect/finalize")]
        public async Task<IActionResult> Finalize()
        {
            // TODO: Finalize command
            if (User.Identity == null)
                return await Task.FromResult(BadRequest("User did not finish sign-in"));

            // Get claims
            var id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            await _mediator.Send(new CreateUserCommand(id, name, null, null));

            return await Task.FromResult(Ok(new { User.Identity.AuthenticationType, id, name }));
        }
    }
}