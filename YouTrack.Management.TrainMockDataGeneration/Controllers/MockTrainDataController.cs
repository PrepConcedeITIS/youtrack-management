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
            const string path = "mockdata.csv";
            if (!System.IO.File.Exists(path))
            {
                var result = _mockDataGenerationService.Handle().ToList();
                using (var streamWriter = new StreamWriter(path))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    await csvWriter.WriteRecordsAsync(result);
                }
            }

            return File(await System.IO.File.ReadAllBytesAsync(path), "text/csv", path);
        }
    }
}