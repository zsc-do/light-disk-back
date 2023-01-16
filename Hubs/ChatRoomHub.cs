using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using lightDiskBack.Models;

namespace lightDiskBack.Hubs
{
	[Authorize]
    public class ChatRoomHub : Hub
	{
	


        public async Task<string> SendPrivateMessage(string destUserName, string message)
        {

            SysUser destUser = idDBContext.Users.Where(a => a.UserName == destUserName).ToList().First();


            if (destUser == null)
            {
                return "DestUserNotFound";
            }
            string destUserId = destUser.Id.ToString();
            SysUser srcUser = idDBContext.Users.Where(a => a.UserName == "zsc").ToList().First();

            string time = DateTime.Now.ToShortTimeString();
            await this.Clients.User(destUserId).SendAsync("ReceivePrivateMessage",
                srcUser.UserName, time, message);
            return "ok";
        }


        private readonly IdDBContext idDBContext;

        public ChatRoomHub(IdDBContext idDBContext)
        {
            this.idDBContext = idDBContext;

        }
    }



}
