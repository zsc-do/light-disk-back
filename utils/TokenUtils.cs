using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using lightDiskBack.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lightDiskBack.utils
{
    public class TokenUtils
    {

        private readonly RoleManager<SysRole> roleManager;
        private readonly UserManager<SysUser> userManager;
        private readonly IdDBContext idDBContext;
        private readonly IDistributedCache distCache;

        public TokenUtils(RoleManager<SysRole> roleManager, UserManager<SysUser> userManager,
                                IdDBContext idDBContext, IDistributedCache distCache
                                 )
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.idDBContext = idDBContext;
            this.distCache = distCache;

        }


        public async Task<string> generateTokenAsync(SysUser user)
        {


            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var roles = await userManager.GetRolesAsync(user);
            
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            string jwtToken = BuildToken(claims);

            return jwtToken;
        }

        private static string BuildToken(IEnumerable<Claim> claims)
        {
            DateTime expires = DateTime.Now.AddSeconds(86400);
            byte[] keyBytes = Encoding.UTF8.GetBytes("fasdfad&9045dafz222#fadpio@0232");
            var secKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(secKey,
                SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(expires: expires,
                signingCredentials: credentials, claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

       
    }
}
