﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Unidash.Auth.Application.DataModels;
using Unidash.Events.Auth;

namespace Unidash.Auth.Application.Features.Users.Requests
{
    public class RegisterUserRequest : IRequest<IActionResult>
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public class Handler : IRequestHandler<RegisterUserRequest, IActionResult>
        {
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly IBus _bus;

            public Handler(IMapper mapper,
                UserManager<User> userManager,
                IBus bus)
            {
                _mapper = mapper;
                _userManager = userManager;
                _bus = bus;
            }

            public async Task<IActionResult> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
            {
                var user = _mapper.Map<User>(request);

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    var @event = _mapper.Map<UserRegisteredEvent>(user);
                    await _bus.Publish(@event, cancellationToken);

                    return new CreatedResult(string.Empty, string.Empty);
                }

                throw new Exception(result.Errors.First().Description);
            }
        }
    }
}
