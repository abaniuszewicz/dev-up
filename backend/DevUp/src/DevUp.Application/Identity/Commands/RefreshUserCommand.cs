﻿using MediatR;

namespace DevUp.Application.Identity.Commands
{
    public sealed class RefreshUserCommand : IRequest
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
        public string DeviceId { get; init; }
        public string DeviceName { get; init; }
    }
}
