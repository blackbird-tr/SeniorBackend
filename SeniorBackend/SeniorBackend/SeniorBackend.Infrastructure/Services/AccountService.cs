using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SeniorBackend.Core.DTOs.Account;
using SeniorBackend.Core.Entities;
using SeniorBackend.Core.Interfaces;
using SeniorBackend.Core.Interfaces.Repositories;
using SeniorBackend.Core.Settings;
using SeniorBackend.Infrastructure.Context;
using SeniorBackend.Infrastructure.Helpers;
using SeniorBackend.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler; 
        private readonly ApplicationDbContext dbContext;
        private readonly IGenericRepository<AppUser> _genericRepositoryAsync;



        public AccountService(UserManager<AppUser> userManager, 
          IOptions<JWTSettings> jwtSettings, 
          SignInManager<AppUser> signInManager, 
          ApplicationDbContext dbContext,
          IGenericRepository<AppUser> genericRepositoryAsync)
        {
            _userManager = userManager; 
            _jwtSettings = jwtSettings.Value; 
            _signInManager = signInManager; 
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            this.dbContext = dbContext;
            this._genericRepositoryAsync = genericRepositoryAsync;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
           var user=await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception($"Email No Accounts Registered with {request.Email}.");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);

            if(!result.Succeeded) { throw new Exception($"Email Invalid Credentials for '{request.Email}'.");
            }
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName; 
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            dbContext.RefreshTokens.Add(new RefreshToken
            {

                Token = refreshToken.Token,
                Expires = refreshToken.Expires,
                Created = refreshToken.Created,
                CreatedByIp = refreshToken.CreatedByIp,
                Revoked = refreshToken.Revoked,
                RevokedByIp = refreshToken.RevokedByIp,
                ReplacedByToken = refreshToken.ReplacedByToken,
                UserID = user.Id,
            });
            dbContext.SaveChanges();
            return response;
        }

        public Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(RequestRefreshToken requestRefreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new Exception($"UserName Username '{request.UserName}' is already taken.");
            }

            var user = new AppUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName, 
                PhoneNumber = request.PhoneNumber
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null) throw new Exception($"Email Email {request.Email} is already registered.");

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                Exception dataAnnotationException = null; ;
                foreach (var item in result.Errors)
                {
                    dataAnnotationException = new Exception($"Password, {item.Description}");
                }

                throw dataAnnotationException;
            }

            User newuser = new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName, 
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
            };
            await dbContext.Set<User>().AddAsync(newuser);  

            //TODO: Attach Email Service here and configure it via appsettings
            // await _emailService.SendAsync(new Core.DTOs.Email.EmailRequest() { From = "csartarge@gmail.com", To = user.Email, Body = $"Your confirm code is - {code}", Subject = "Confirm Registration" });


            return new RegisterResponse { UserId = newuser.Id, UserName = newuser.UserName };
        }

        public Task<ValidateRefreshTokenResponse> ValidateRefreshToken(string userId, string token)
        {
            throw new NotImplementedException();
        }




        private async Task<JwtSecurityToken> GenerateJWToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
             
 

            string ipAddress = IpHelper.GetIpAddress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }
        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

    }
}
