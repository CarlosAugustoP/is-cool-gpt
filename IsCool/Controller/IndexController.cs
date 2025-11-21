using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace IsCool.Controller
{
    [ApiController]
    [Route("api/index")]
    public class IndexController : IsCoolController
    {
        [HttpGet]
        public IActionResult Heartbeat()
        {
            return Ok("Hello from IndexController");
        }
    }
}