using Microsoft.AspNetCore.Mvc;
using SeniorBackend.Core.DTOs.Account;
using SeniorBackend.Core.Interfaces;
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
            return Ok(await _accountService.AuthenticateAsync(request, GenerateIPAddress()));
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
        [HttpPost("Reset-Password")]
        [ProducesResponseType(typeof(ChangePasswordResponse), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request,string userID)
        {
            return Ok(await _accountService.ChangePassword(request,userID));
        }



        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

    }

}
