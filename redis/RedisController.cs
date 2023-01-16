using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace lightDiskBack.redis
{
    [Route("api/redis")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redis;
        public RedisController(RedisHelper client)
        {
            _redis = client.GetDatabase();
        }

        [HttpGet]
        public string Get()
        {
            // 往Redis里面存入数据
            /* _redis.StringSet("Name", "Tom");
             // 从Redis里面取数据
             string name = _redis.StringGet("Name");*/

            _redis.StringSetBit("haha", 2, true);
            return "hh";
        }
    }
}
