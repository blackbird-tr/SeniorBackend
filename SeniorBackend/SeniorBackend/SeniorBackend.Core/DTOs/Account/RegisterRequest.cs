using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.DTOs.Account
{
    public class RegisterRequest
    {
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }
         

        [Required][EmailAddress] public string Email { get; set; }

        [Required][Phone] public string PhoneNumber { get; set; }


        [Required][MinLength(6)] public string UserName { get; set; }

        [Required][MinLength(6)] public string Password { get; set; }

        [Required][Compare("Password")] public string ConfirmPassword { get; set; }
    }
}