using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
                        
            if (!result.Succeeded) { throw new Exception($"Email Invalid Credentials for '{request.Email}'.");
            }
            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!emailConfirmed)
            {
                throw new Exception($"Email Not confirmed {request.Email}.");
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

        public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { throw new Exception("User not found"); }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded) { throw new Exception("There occur an error "); }
            return new ChangePasswordResponse(){
                UserId = userId, Message = "Succesful"
            };
        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(string email, string code)
        {
            var user=await _userManager.FindByEmailAsync(email);
            if (user == null) { throw new Exception("user connot find"); }
            var result =await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new Exception("cannot approved");
            }
            return new ConfirmEmailResponse() { Message="succesful" , UserId = user.Id };
        }

        public async Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(RequestRefreshToken requestRefreshToken)
        {
            ExchangeRefreshTokenResponse response;
            try
            {
                //Getting user claims from token
                ClaimsPrincipal claimsPrincipal = ValidateToken(requestRefreshToken.AccessToken, new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ValidateLifetime = false // we check expired tokens here
                });
                // invalid token/signing key was passed and we can't extract user claims
                if (claimsPrincipal == null)
                    throw new Exception("TokenError");
                //Getting user from repository

                //var user = await _userManager.GetUserAsync(claimsPrincipal);
                var myuserId = dbContext.RefreshTokens.Where(a => a.Token == requestRefreshToken.RefreshToken).Select(f => f.UserID).FirstOrDefault();
                AppUser user = await _userManager.FindByIdAsync(myuserId);
                //generating new access token
                var jwtToken = await GenerateJWToken(user);
                //generating new refresh token
                var refreshToken = RandomTokenString();
                // delete the refresh token that exchanged
                var deleterefreshToken = dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == requestRefreshToken.RefreshToken);

                if (deleterefreshToken != null)
                {
                    dbContext.RefreshTokens.Remove(deleterefreshToken);
                    dbContext.SaveChanges();
                }


                // add the new one
                dbContext.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    UserID = user.Id,
                });
                dbContext.SaveChanges();
                await _genericRepositoryAsync.UpdateAsync(user);
                response = new ExchangeRefreshTokenResponse { Message = refreshToken };

            }
            catch (Exception ex)
            {
                response = new ExchangeRefreshTokenResponse { Message = ex.Message };
            }
            return response;

        }
        public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters)
        {
            try
            {
                var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                return principal;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return null;
            }
        }

        public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest model)
        {
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user == null) { throw new Exception("user not found"); }
            
            var response=await _userManager.ResetPasswordAsync(user,model.Token,model.NewPassword);
            if (!response.Succeeded) { throw new Exception("occured an error"); }

            return new ForgotPasswordResponse() { Message = "Succes" };
        }

        public async Task<ForgotPasswordResponse> GenerateForgotPasswordToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) { throw new Exception("user  cannot find"); }

            var response=await _userManager.GeneratePasswordResetTokenAsync(user);
            return new ForgotPasswordResponse() { Message = response };
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

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            return new RegisterResponse { UserId = newuser.Id, UserName = newuser.UserName,Message=code };
        }

        public async Task<ResendEmailConfirmCodeResponse> ResendConfirmEmailCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("user not found");
            var isConfirmed=await _userManager.IsEmailConfirmedAsync(user);
            if (isConfirmed) { throw new Exception("email already confirmed"); }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            return new ResendEmailConfirmCodeResponse { Message=code};
        }




        public async Task<ValidateRefreshTokenResponse> ValidateRefreshToken(string userId, string token)
        { 
            RefreshToken refreshToken = dbContext.RefreshTokens.Where(r => r.Token == token).FirstOrDefault();

            if (refreshToken.UserID == userId && refreshToken.Token == token && refreshToken.IsActive)
            {
                return new ValidateRefreshTokenResponse { UserId = userId, Message = "Succeded" };
            }
            else
            {
                return new ValidateRefreshTokenResponse { UserId = userId, Message = "Refresh Token not Invalid" };
            }

            throw new InvalidOperationException();
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
