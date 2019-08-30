using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tables/booking")]
    [ApiController]
    public class TableBookingController : ControllerBase
    {
        [HttpPost]
        public Guid BookTable()
        {
            return new Guid();
        }
    }
}
