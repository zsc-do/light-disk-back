using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using lightDiskBack.redis;
using lightDiskBack.Vo;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using lightDiskBack.Models;

namespace lightDiskBack.Controllers.wp
{
    public class SignController : ControllerBase
    {
        private readonly IDatabase _redis;

        private readonly IdDBContext idDBContext;

        public SignController(RedisHelper client, IdDBContext idDBContext)
        {
            _redis = client.GetDatabase();
            this.idDBContext = idDBContext;
        }

        [Authorize]
        public JsonResult sign()
        {
            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int userId = userId = int.Parse(userId1);


            SignVo signInfo = getSignInfo(userId);

            if(signInfo.isSign == 1)
            {
                return new JsonResult("403");
            }

            DateTime dateTime = DateTime.Now;
            string now = dateTime.ToString("yyyy-MM");
            int toDay = dateTime.Day;

            _redis.StringSetBit(now + "-" + userId, toDay - 1, true);

            int hasSignCount = signInfo.signCount + 1;

            var storage = idDBContext.Storage.Where(a => a.userId == userId).Single();

            if (hasSignCount <= 4)
            {
                storage.simpleSize = (int.Parse(storage.simpleSize) + 52428800).ToString();

            }else if(hasSignCount == 5)
            {
                storage.simpleSize = (int.Parse(storage.simpleSize) + 8589934592).ToString();
            }

            idDBContext.SaveChanges();

            return new JsonResult("ok");
        }

        [Authorize]
        public JsonResult signSiuation()
        {
            String userId1 = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            int userId = userId = int.Parse(userId1);

            SignVo signInfo = getSignInfo(userId);


            return new JsonResult(signInfo);
        }

        private SignVo getSignInfo(int userId)
        {


            DateTime dateTime = DateTime.Now;
            string now = dateTime.ToString("yyyy-MM");
            int lastDay = dateTime.Day - 1;

            int signCount = 0;
            SignVo signVo = new SignVo();

            if (lastDay == -1)
            {
                signVo.isSign = 0;
                signVo.signCount = 0;
                return signVo;
            }

            while (_redis.StringGetBit(now + "-" + userId, lastDay - 1) == true)
            {
                signCount++;

                lastDay--;

                if (lastDay < 1) break;
            }

            signCount = signCount % 5;


            //今天签了的情况
            if (_redis.StringGetBit(now + "-" + userId, dateTime.Day - 1) == true)
            {
                signVo.isSign = 1;
                signVo.signCount = signCount + 1;
                return signVo;
            }

            signVo.isSign = 0;
            signVo.signCount = signCount;

            return signVo;
        }
    }
}
