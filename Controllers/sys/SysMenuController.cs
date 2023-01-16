using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lightDiskBack.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lightDiskBack.Controllers.sys
{
    public class SysMenuController : Controller
    {

		private readonly RoleManager<SysRole> roleManager;
		private readonly UserManager<SysUser> userManager;
		private readonly IdDBContext idDBContext;
		public SysMenuController(RoleManager<SysRole> roleManager, UserManager<SysUser> userManager,
								IdDBContext idDBContext)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.idDBContext = idDBContext;

		}


		public async Task<JsonResult> getAuthMenuAsync()
        {
            var authUser = idDBContext.Users
                 .Include(a => a.Role)
                 .ThenInclude(s => s.SysMenus).ToList().First();


            List<SysMenu> AuthMenus = new List<SysMenu>();

            foreach (var Role in authUser.Role)
            {
                foreach(var menu in Role.SysMenus)
                {
                    AuthMenus.Add(menu);
                }
            }


            ArrayList FinalMenus = new ArrayList();

            foreach (SysMenu menu in AuthMenus)
            {

                if (menu.ParentId.Equals(0))
                {

                    FinalMenus.Add(menu);
                    menu.ChildMenu = new ArrayList();

                    foreach (SysMenu menu1 in AuthMenus)
                    {

                        if (menu1.ParentId.Equals(menu.MenuId))
                        {
                            menu.ChildMenu.Add(menu1);


                            menu1.ChildMenu = new ArrayList();
                            foreach (SysMenu menu2 in AuthMenus)
                            {
                                if (menu2.ParentId.Equals(menu1.MenuId))
                                {
                                    menu1.ChildMenu.Add(menu2);
                                }
                            }
                        }
                    }
                }

            }
            return new JsonResult(FinalMenus);
        }


        public IActionResult menuView()
        {

            return View();
        }

        public IActionResult GetAllTreeMenu()
        {

            var menus = idDBContext.SysMenus.Where(a => a.DelFlag == "1").ToList();

            ArrayList FinalMenus = new ArrayList();

            foreach (SysMenu menu in menus)
            {

                if (menu.ParentId.Equals(0))
                {

                    FinalMenus.Add(menu);
                    menu.ChildMenu = new ArrayList();

                    foreach (SysMenu menu1 in menus)
                    {

                        if (menu1.ParentId.Equals(menu.MenuId))
                        {
                            menu.ChildMenu.Add(menu1);


                            menu1.ChildMenu = new ArrayList();
                            foreach (SysMenu menu2 in menus)
                            {
                                if (menu2.ParentId.Equals(menu1.MenuId))
                                {
                                    menu1.ChildMenu.Add(menu2);
                                }
                            }
                        }
                    }
                }

            }
            return new JsonResult(FinalMenus);
        }


        public IActionResult MenuAddPage()
        {

            return View();
        }


        public JsonResult GetAllML()
        {
            var mmenus = idDBContext.SysMenus.Where(a => a.MenuType == "M").ToList();

             return new JsonResult(mmenus);
        }


        public JsonResult GetAllCD()
        {
            var cmenus = idDBContext.SysMenus.Where(a => a.MenuType == "C").ToList();

            return new JsonResult(cmenus);
        }


        public JsonResult MenuAdd(
            [FromForm(Name = "menuName")]String menuName,
            [FromForm(Name = "menuUrl")] String menuUrl,
            [FromForm(Name = "menuPerms")] String menuPerms,
            [FromForm(Name = "cat")] String cat,
            [FromForm(Name = "Pmenu")] String Pmenu)
        {

            SysMenu menu = new SysMenu();
            menu.MenuName = menuName;
            menu.MenuUrl = menuUrl;
            menu.MenuPerms = menuPerms;
            menu.MenuType = cat;
            menu.ParentId = int.Parse(Pmenu);
            menu.DelFlag = "1";

            idDBContext.SysMenus.Add(menu);

            idDBContext.SaveChanges();

            return new JsonResult("");
        }


        public IActionResult MenuEditPage()
        {
            return View();
        }

        public JsonResult GetMenu([FromQuery(Name = "menuId")] String menuId)
        {
            int menuintId = int.Parse(menuId);
            var menu = idDBContext.SysMenus.Where(a => a.MenuId == menuintId).ToList();
            return new JsonResult(menu);
        }


        public JsonResult MenuEdit(
            [FromForm(Name = "menuName")] String menuName,
            [FromForm(Name = "menuUrl")] String menuUrl,
            [FromForm(Name = "menuPerms")] String menuPerms,
            [FromForm(Name = "cat")] String cat,
            [FromForm(Name = "Pmenu")] String Pmenu,
            [FromQuery(Name = "menuId")] String menuId)
        {

            var menu = idDBContext.SysMenus.Where(a => a.MenuId == int.Parse(menuId)).Single();
            menu.MenuName = menuName;
            menu.MenuUrl = menuUrl;
            menu.MenuPerms = menuPerms;
            menu.MenuType = cat;
            menu.ParentId = int.Parse(Pmenu);
            menu.DelFlag = "1";


            idDBContext.SaveChanges();

            return new JsonResult("");
        }


        public JsonResult MenuRemove([FromQuery(Name = "menuId")] String menuId)
        {
            var menu = idDBContext.SysMenus.Where(a => a.MenuId == int.Parse(menuId)).Single();

            idDBContext.SysMenus.Remove(menu);
            idDBContext.SaveChanges();

            return new JsonResult("");
        }
    }
}
