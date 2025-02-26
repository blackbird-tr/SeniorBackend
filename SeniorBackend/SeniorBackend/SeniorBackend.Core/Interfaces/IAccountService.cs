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
            
        Task<ValidateRefreshTokenResponse> ValidateRefreshToken(string userId, string token);

        Task<ExchangeRefreshTokenResponse> ExchangeRefreshToken(RequestRefreshToken requestRefreshToken);
    }
}
