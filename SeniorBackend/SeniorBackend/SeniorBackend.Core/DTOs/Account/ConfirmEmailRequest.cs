﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.DTOs.Account
{
    public class ConfirmEmailRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
