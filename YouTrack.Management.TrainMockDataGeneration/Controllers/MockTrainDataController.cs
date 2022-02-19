using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;

namespace YouTrack.Management.TrainMockDataGeneration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockTrainDataController : ControllerBase
    {
        private readonly MockDataGenerationService _mockDataGenerationService;
        public MockTrainDataController(MockDataGenerationService mockDataGenerationService)
        {
            _mockDataGenerationService = mockDataGenerationService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_mockDataGenerationService.Handle());
        }        
        [HttpGet("csv")]
        public async Task<IActionResult> Csv()
        {
            var result = _mockDataGenerationService.Handle().ToList();
            byte[] bytes = null;
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    await csvWriter.WriteRecordsAsync(result);
                } 

                bytes = memoryStream.ToArray();
            }
            
            return File(bytes, "text/csv");
        }
    }
}