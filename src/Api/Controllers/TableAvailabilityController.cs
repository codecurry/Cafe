using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ddd.Api;

namespace Ddd.Api.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TableAvailabilityController : ControllerBase
    {
        [HttpGet("available")]
        public async Task<List<string>> Get()
        {
            return await Task.Run(() => TableList.AvailableTables); 
        }
        [HttpGet("open")]
        public async Task<List<string>> GetOpenTables()
        {
            return await Task.Run(() => TableList.OpenTables ?? new List<string>()); 
        }
    }
}
