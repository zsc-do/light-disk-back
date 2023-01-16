using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using lightDiskBack.Controllers.login;
using lightDiskBack.Dto;
using lightDiskBack.Models;
using lightDiskBack.utils;
using lightDiskBack.Vo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace lightDiskBack.Controllers.oauth
{
    public class OauthController : Controller
    {

        private readonly IdDBContext idDBContext;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly TokenUtils tokenUtils;


        public OauthController(IdDBContext idDBContext, IHttpClientFactory httpClientFactory,
                               TokenUtils tokenUtils)
        {
            this.idDBContext = idDBContext;
            this._httpClientFactory = httpClientFactory;
            this.tokenUtils = tokenUtils;
        }


        public async Task<JsonResult> getTokenAsync([FromQuery(Name = "code")] String code)
        {

            string clientId = "Iv1.919336f5c76c06a2";
            string client_secret = "9b1c815ee7abb38e0d8616c05aae25981a6249b5";

            var httpRequestMessage = new HttpRequestMessage(
                 HttpMethod.Get,
                "https://github.com/login/oauth/access_token?client_id=" + clientId + "&client_secret=" + client_secret + "&code=" + code)
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/json" },
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            AccessToken access_token = null;
            GithubUser githubUser = null;
            string token = null;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentString = await httpResponseMessage.Content.ReadAsStringAsync();

                access_token = JsonSerializer.Deserialize<AccessToken>(contentString);

                var httpRequestMessage2 = new HttpRequestMessage(
                    HttpMethod.Get,
                    "https://api.github.com/user")
                {
                    Headers =
                    {
                        { HeaderNames.Accept, "application/json" },
                        { HeaderNames.Authorization,"bearer " + access_token.access_token },
                        {HeaderNames.UserAgent, "HttpRequestsSample" }
                    }
                };

                var httpResponseMessage2 = await httpClient.SendAsync(httpRequestMessage2);
                string contentString2 = await httpResponseMessage2.Content.ReadAsStringAsync();

                githubUser = JsonSerializer.Deserialize<GithubUser>(contentString2);
                SysUser user = null;
                try
                {
                    user =  idDBContext.Users.SingleOrDefault(a => a.githubId == githubUser.id.ToString());
                }catch(Exception e)
                {
                    return new JsonResult(user);
                }

                if(user == null)
                {
                    SysUser sysUser = new SysUser();
                    sysUser.UserName = githubUser.login;
                    sysUser.NormalizedUserName = githubUser.login;
                    sysUser.EmailConfirmed = false;
                    sysUser.PasswordHash = "";
                    sysUser.PhoneNumberConfirmed = false;
                    sysUser.TwoFactorEnabled = false;
                    sysUser.LockoutEnabled = false;
                    sysUser.AccessFailedCount = 0;
                    sysUser.githubId = githubUser.id.ToString();

                    idDBContext.Users.Add(sysUser);
                    idDBContext.SaveChanges();

                    WpFile rootFolder = new WpFile();
                    rootFolder.delFlag = "0";
                    rootFolder.fileName = "";
                    rootFolder.isFolder = "1";
                    rootFolder.userId = sysUser.Id;
                    rootFolder.filePath = "";

                    idDBContext.wpFile.Add(rootFolder);


                    Storage storage = new Storage();
                    storage.isMember = "0";
                    storage.memberSize = "0";
                    storage.simpleSize = "1073741824";//1GB
                    storage.storageSize = "0";
                    storage.userId = sysUser.Id;

                    idDBContext.Storage.Add(storage);

                    idDBContext.SaveChanges();

                    user = sysUser;
                }

                token = await tokenUtils.generateTokenAsync(user);

                

            }


            return new JsonResult(token);

        }
     }
}
