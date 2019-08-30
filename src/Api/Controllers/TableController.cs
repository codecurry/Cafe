using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ddd.Commands;

namespace Api.Controllers
{
    [Route("api/tables")]
    [ApiController]
    public class TableController : ControllerBase
    {
        [HttpPost("open")]
        public Task OpenTable(OpenTable cmd)
        {
            return null;
        }
    }
}
