﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.V1.Controllers.Identity.Requests;
using DevUp.Api.V1.Controllers.Identity.Responses;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Controllers.Identity
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public IdentityController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = _mapper.Map<Username>(request);
                var password = _mapper.Map<Password>(request);
                var device = _mapper.Map<Device>(request.Device);

                var result = await _identityService.RegisterAsync(username, password, device, cancellationToken);
                var response = _mapper.Map<IdentityResponse>(result);
                return Ok(response);
            }
            catch (DomainException exception)
            {
                var response = _mapper.Map<IdentityResponse>(exception);
                return BadRequest(response);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = _mapper.Map<Username>(request);
                var password = _mapper.Map<Password>(request);
                var device = _mapper.Map<Device>(request.Device);

                var result = await _identityService.LoginAsync(username, password, device, cancellationToken);
                var response = _mapper.Map<IdentityResponse>(result);
                return Ok(response);
            }
            catch (DomainException exception)
            {
                var response = _mapper.Map<IdentityResponse>(exception);
                return BadRequest(response);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = _mapper.Map<Token>(request);
                var refreshToken = _mapper.Map<RefreshToken>(request);
                var device = _mapper.Map<Device>(request.Device);

                var result = await _identityService.RefreshAsync(token, refreshToken, device, cancellationToken);
                var response = _mapper.Map<IdentityResponse>(result);
                return Ok(response);
            }
            catch (DomainException exception)
            {
                var response = _mapper.Map<IdentityResponse>(exception);
                return BadRequest(response);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }
    }
}
