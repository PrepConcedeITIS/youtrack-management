using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace YouTrack.Management.TrainMockDataGeneration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockTrainDataController : ControllerBase
    {
        public MockTrainDataController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}