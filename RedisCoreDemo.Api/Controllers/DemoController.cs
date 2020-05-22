using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace RedisCoreDemo.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DemoController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;

        public DemoController(ILogger<DemoController> logger, IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Set(int id)
        {
            await _distributedCache.SetStringAsync($"order_{id}"
                , id.ToString()
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10)
                });
            return Ok("success");
        }
    }
}
