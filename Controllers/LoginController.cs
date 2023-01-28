using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using lightDiskBack.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace lightDiskBack.Controllers.login
{
	public class LoginController : Controller
	{
		private readonly RoleManager<SysRole> roleManager;
		private readonly UserManager<SysUser> userManager;
		private readonly IdDBContext idDBContext;
		private readonly IDistributedCache distCache;
		public LoginController(RoleManager<SysRole> roleManager, UserManager<SysUser> userManager,
								IdDBContext idDBContext, IDistributedCache distCache)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.idDBContext = idDBContext;
			this.distCache = distCache;

		}

		public IActionResult login()
		{
			return View("login");
		}


		[HttpPost]
		public async Task<IActionResult> doLogin([FromBody]LoginRequest req,
					[FromServices] IOptions<JWTOptions> jwtOptions)
		{


			string userName = req.UserName;
			string password = req.Password;
			var user = await userManager.FindByNameAsync(userName);
			if (user == null)
			{
                return new JsonResult("500");
            }
			var success = await userManager.CheckPasswordAsync(user, password);
			if (!success)
			{
                return new JsonResult("500");
            }
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			var roles = await userManager.GetRolesAsync(user);
			foreach (string role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			string jwtToken = BuildToken(claims, jwtOptions.Value);




			return new JsonResult(jwtToken);
		}

		private static string BuildToken(IEnumerable<Claim> claims, JWTOptions options)
		{
			DateTime expires = DateTime.Now.AddSeconds(options.ExpireSeconds);
			byte[] keyBytes = Encoding.UTF8.GetBytes(options.SigningKey);
			var secKey = new SymmetricSecurityKey(keyBytes);
			var credentials = new SigningCredentials(secKey,
				SecurityAlgorithms.HmacSha256Signature);
			var tokenDescriptor = new JwtSecurityToken(expires: expires,
				signingCredentials: credentials, claims: claims);
			return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
		}



		[HttpPost]
		public async Task<IActionResult> register([FromBody] LoginRequest req,
					[FromServices] IOptions<JWTOptions> jwtOptions)
		{


			string userName = req.UserName;
			string password = req.Password;

			SysUser user = new SysUser();
			user.UserName = userName;


			var r = await userManager.CreateAsync(user, password);


            if (!r.Succeeded)
            {
				return BadRequest(r.Errors);
            }

			WpFile rootFolder = new WpFile();
			rootFolder.delFlag = "0";
			rootFolder.fileName = "";
			rootFolder.isFolder = "1";
			rootFolder.userId = user.Id;
			rootFolder.filePath = "";

			idDBContext.Add(rootFolder);


			Storage storage = new Storage();
			storage.isMember = "0";
			storage.memberSize = "0";
			storage.simpleSize = "1073741824";//1GB
			storage.storageSize = "0";
			storage.userId = user.Id;

			idDBContext.Add(storage);


			idDBContext.SaveChanges();


			return new JsonResult("");
		}


	}
}
