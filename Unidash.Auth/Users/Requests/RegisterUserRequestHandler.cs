﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Unidash.Auth.Users.Commands;
using Unidash.Auth.Users.Queries;
using Unidash.Core.Security;

namespace Unidash.Auth.Users.Requests
{
    public class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, IActionResult>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly JwtTokenService _jwtTokenService;

        public RegisterUserRequestHandler(IMediator mediator, IMapper mapper,
            JwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<IActionResult> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            // Check whether user already exists
            var user = await _mediator.Send(new FindUserByEmailQuery(request.EmailAddress), cancellationToken);
            if (user != null)
                return new BadRequestObjectResult("User already exists");

            // Create user
            var id = Guid.NewGuid().ToString();
            await _mediator.Send(new CreateUserCommand(id,
                request.DisplayName,
                request.EmailAddress,
                request.Password));

            // Create JWT
            var token = _jwtTokenService.WriteToken(new Dictionary<string, string>
            {
                {ClaimTypes.Name, id}
            }, new JwtTokenMeta
            {
                Audience = "unidash",
                Issuer = "unidash",
                ExpiresAt = DateTime.Now.AddDays(7)
            });

            // Return action result
            return new OkObjectResult(token);
        }
    }
}