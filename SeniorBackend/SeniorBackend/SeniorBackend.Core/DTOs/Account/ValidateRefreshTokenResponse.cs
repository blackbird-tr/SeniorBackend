using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.DTOs.Account
{
    public class ValidateRefreshTokenResponse
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }
}
