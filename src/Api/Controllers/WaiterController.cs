using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ddd.Api;

namespace Ddd.Api.Controllers
{
    [Route("api/waiters")]
    [ApiController]
    public class WaitersController : ControllerBase
    {
        [HttpGet]
        public async Task<List<string>> Get()
        {
            return await Task.Run(() => Staff.Waiters); 
        }
    }
}
