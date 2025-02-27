using SeniorBackend.Core.DTOs.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.Interfaces
{
    public interface IAccountService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request, string ipAddress);

        Task<ConfirmEmailResponse> ConfirmEmailAsync(string email, string code);
        Task<ResendEmailConfirmCodeResponse> ResendConfirmEmailCodeAsync(string email);
        Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest model);
        Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest model);
        Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest model, string userId);
        Task<ValidateRefreshTokenResponse> ValidateRefreshToken(string userId, string token);

        Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(RequestRefreshToken requestRefreshToken);
    }
}
