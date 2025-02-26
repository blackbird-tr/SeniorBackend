using Microsoft.AspNetCore.Identity;
using SeniorBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorBackend.Infrastructure.Models
{
    public class AppUser:IdentityUser
    {
        public AppUser()
        {
            RefreshTokens = new HashSet<RefreshToken>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public DateTime DateCreated { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }


        public bool OwnsToken(string token)
        {
            return this.RefreshTokens?.FirstOrDefault(x => x.Token == token) != null;
        }
        public void AddRefreshToken(string token, string userId, string remoteIpAddress, double daysToExpire = 5)
        {
            RefreshTokens.Add(new RefreshToken(token, DateTime.UtcNow.AddDays(daysToExpire), userId, remoteIpAddress));
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            RefreshTokens.Remove(RefreshTokens.First(t => t.Token == refreshToken));
        }
        public bool HasValidRefreshToken(string refreshToken)
        {
            return RefreshTokens.Any(rt => rt.Token == refreshToken && rt.IsActive);
        }
    }
}
