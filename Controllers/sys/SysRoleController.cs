using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lightDiskBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lightDiskBack.Controllers.sys
{
    public class SysRoleController : Controller
    {

		private readonly RoleManager<SysRole> roleManager;
		private readonly UserManager<SysUser> userManager;
		private readonly IdDBContext idDBContext;
		public SysRoleController(RoleManager<SysRole> roleManager, UserManager<SysUser> userManager,
								IdDBContext idDBContext)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.idDBContext = idDBContext;

		}


		public IActionResult roleView()
		{

			return View();
		}

		public JsonResult RoleList()
		{

			var roles = idDBContext.Roles.Where(a => a.Id != 0).ToList();

			return new JsonResult(roles);
		}


		public IActionResult RoleAddPage()
		{

			return View();
		}

		public JsonResult RoleAdd(
			[FromForm(Name = "roleName")] String roleName,
			[FromForm(Name = "checkedMenus")] String checkedMenus)
		{

			checkedMenus = checkedMenus.Replace("[", "").Replace("]", "");
            string[] vs = checkedMenus.Split(",");

            SysRole sysRole = new SysRole();
			sysRole.Name = roleName;

			idDBContext.Roles.Add(sysRole);

			idDBContext.SaveChanges();

			foreach(String s in vs)
            {
				idDBContext.Database.ExecuteSqlRaw($"insert into sys_role_menu (SysRolesId,SysMenusMenuId) values ({sysRole.Id}, {int.Parse(s)})");
            }


			return new JsonResult("");
		}

		public IActionResult RoleEditPage()
        {
			return View();
        }

		public JsonResult GetRole([FromQuery(Name = "roleId")] String roleId)
		{
			int roleintId = int.Parse(roleId);
			var role = idDBContext.Roles.Where(a => a.Id == roleintId).ToList();
			return new JsonResult(role);
		}


		public JsonResult RoleEdit(
			[FromQuery(Name ="roleId")] String roleId,
			[FromForm(Name = "roleName")] String roleName,
			[FromForm(Name = "checkedMenus")] String checkedMenus)
		{

			checkedMenus = checkedMenus.Replace("[", "").Replace("]", "");
			string[] vs = checkedMenus.Split(",");

			var role = idDBContext.Roles.Where(a => a.Id == int.Parse(roleId)).ToList().First();
			role.Name = roleName;

			idDBContext.SaveChanges();

			idDBContext.Database.ExecuteSqlRaw($"delete from sys_role_menu where SysRolesId =  {int.Parse(roleId)}");



			foreach (String s in vs)
			{
				idDBContext.Database.ExecuteSqlRaw($"insert into sys_role_menu (SysRolesId,SysMenusMenuId) values ({role.Id}, {int.Parse(s)})");
			}


			return new JsonResult("");
		}


		public JsonResult RoleRemove([FromQuery(Name = "roleId")] String roleId)
        {
			var role= idDBContext.Roles.Where(a => a.Id == int.Parse(roleId)).Single();

			idDBContext.Roles.Remove(role);
			idDBContext.SaveChanges();

			idDBContext.Database.ExecuteSqlRaw($"delete from sys_role_menu where SysRolesId =  {int.Parse(roleId)}");


			return new JsonResult("");
		}
	}
}
