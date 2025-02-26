using SeniorBackend.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Core.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool isExpired => DateTime.UtcNow >= Expires;
        public DateTime  Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !isExpired;

        public string RemoteIpAdress { get; private set; }
        public string UserID { get; set; }
        public AppUser User { get; set; }

        public RefreshToken(string token, DateTime expires, string userID, string remoteIpAdress)
        {

            Token = token;
            Expires = expires;
            UserID = userID;
            RemoteIpAdress = remoteIpAdress;
        }

        public RefreshToken() { }

    }
}
