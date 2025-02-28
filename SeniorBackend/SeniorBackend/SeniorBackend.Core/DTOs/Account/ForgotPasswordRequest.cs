using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.DTOs.Account
{
    public class ForgotPasswordRequest
    {
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string Token { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}