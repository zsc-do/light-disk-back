﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lightDiskBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lightDiskBack.Controllers.sys
{
	public class SysUserController : Controller
	{

		private readonly RoleManager<SysRole> roleManager;
		private readonly UserManager<SysUser> userManager;
		private readonly IdDBContext idDBContext;
		public SysUserController(RoleManager<SysRole> roleManager, UserManager<SysUser> userManager,
								IdDBContext idDBContext)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.idDBContext = idDBContext;

		}


		[Authorize]
		public JsonResult userInfo()
		{


			String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
			int userId = userId = int.Parse(userId1);


			var user = idDBContext.Users.Include(s =>s.storage)
				.Where(a => a.Id == userId).Single();

			return new JsonResult(user);
		}

	}
}
