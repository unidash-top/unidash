﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Foodies.Foody.Auth.Users.DataTransfer;
using Foodies.Foody.Auth.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Foodies.Foody.Auth.Users.Requests
{
    public class GetUserRequestHandler : IRequestHandler<GetUserRequest, IActionResult>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetUserRequestHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IActionResult> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _mediator.Send(new FindUserQuery(request.Id), cancellationToken);
            if (user == null)
                return new NotFoundResult();

            var dto = _mapper.Map<UserDto>(user);
            return new OkObjectResult(dto);
        }
    }
}