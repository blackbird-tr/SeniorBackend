﻿using Microsoft.AspNetCore.Mvc;
using SeniorBackend.Core.DTOs.Account;
using SeniorBackend.Core.Interfaces;
using SeniorBackend.Infrastructure.Helpers;
using System.Security.Claims;

namespace SeniorBackend.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(typeof(string), 500)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
            

        }

        [HttpPost("authenticate")] 
        [ProducesResponseType(typeof(AuthenticationResponse), 200)]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _accountService.AuthenticateAsync(request, IpHelper.GetIpAddress()));
        }
        [HttpPost("register")] 
        [ProducesResponseType(typeof(RegisterResponse), 200)]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            return Ok(await _accountService.RegisterAsync(request));
        }

        [HttpPost("Confirm-Email")]
        [ProducesResponseType(typeof(ConfirmEmailResponse), 200)]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            return Ok(await _accountService.ConfirmEmailAsync(request.Email,request.Code));
        }



        [HttpPost("Resend-Confirm-Email-Code")]
        [ProducesResponseType(typeof(ResendEmailConfirmCodeResponse), 200)]
        public async Task<IActionResult> RegisterAsync(ResendEmailConfirmCodeRequest request)
        {
            return Ok(await _accountService.ResendConfirmEmailCodeAsync(request.Email));
        }


        [HttpPost("Change-Password")]
        [ProducesResponseType(typeof(ChangePasswordResponse), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request,string userID)
        {
            return Ok(await _accountService.ChangePassword(request,userID));
        }
        [HttpPost("Reset-Password")]
        [ProducesResponseType(typeof(ForgotPasswordResponse), 200)]
        public async Task<IActionResult> ChangePassword(ForgotPasswordRequest request)
        {
            return Ok(await _accountService.ForgotPassword(request));
        }
        [HttpPost("Reset-Password-Code")]
        [ProducesResponseType(typeof(ForgotPasswordResponse), 200)]
        public async Task<IActionResult> ChangePassword(string email)
        {
            return Ok(await _accountService.GenerateForgotPasswordToken(email));
        }
        [HttpPost("Validation-Refresh-Token")] 
        [ProducesResponseType(typeof(ValidateRefreshTokenResponse), 200)]
        public async Task<IActionResult> ValidationRefreshToken(string userId, string RefreshToken)
        {

            return Ok(await _accountService.ValidateRefreshToken(userId, RefreshToken));
        }

        [HttpPost("ExchangeRefreshToken")] 
        [ProducesResponseType(typeof(ExchangeRefreshTokenResponse), 200)]
        public async Task<ActionResult> RefreshToken([FromBody] RequestRefreshToken requestRefreshToken)
        {
            if (requestRefreshToken == null)
            {

                return new BadRequestResult();
            }

            return Ok(await _accountService.ExchangeRefreshToken(requestRefreshToken,IpHelper.GetIpAddress()));
        }


        
    }

}
