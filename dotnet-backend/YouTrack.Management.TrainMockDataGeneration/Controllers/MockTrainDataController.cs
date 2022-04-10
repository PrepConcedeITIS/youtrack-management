using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.TrainMockDataGeneration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockTrainDataController : ControllerBase
    {
        private const string Path = "mockdata.csv";
        private readonly MockDataGenerationService _mockDataGenerationService;

        public MockTrainDataController(MockDataGenerationService mockDataGenerationService)
        {
            _mockDataGenerationService = mockDataGenerationService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] bool regenerate)
        {
            if (regenerate || !System.IO.File.Exists(Path))
                return Ok(_mockDataGenerationService.Handle());

            using (var reader = new StreamReader(Path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<IssueMlCsv>().ToList();
                return Ok(records);
            }
        }

        [HttpGet("csv")]
        public async Task<IActionResult> Csv()
        {
            if (!System.IO.File.Exists(Path))
            {
                var result = _mockDataGenerationService.Handle().ToList();
                using (var streamWriter = new StreamWriter(Path))
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                {
                    await csvWriter.WriteRecordsAsync(result);
                }
            }

            return File(await System.IO.File.ReadAllBytesAsync(Path), "text/csv", Path);
        }
    }
}