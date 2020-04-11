using FaceSkill.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExtractInformationSkill
{
    public class ExtractInformationSkill
    {
        [FunctionName("extract")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log, ExecutionContext context)
        {
            using var inputStream = new StreamReader(req.Body);
            var requestBody = await inputStream.ReadToEndAsync();
            var data = JToken.Parse(requestBody);

            // Validation
            if (data == null)
            {
                return new BadRequestObjectResult(" Could not find values array");
            }

            if (data["values"]?.FirstOrDefault() == null)
            {
                // It could not find a record, then return empty values array.
                return new BadRequestObjectResult(" Could not find valid records in values array");
            }

            var recordId = data["values"].First()["recordId"]?.ToString();
            if (recordId == null)
            {
                return new BadRequestObjectResult("recordId cannot be null");
            }

            // Creates the response.
            var responseRecord = new WebApiResponseRecord(recordId);
            var response = new WebApiEnricherResponse(responseRecord);

            var imageCaption = data["values"].First()["data"]?["imageCaption"]?.FirstOrDefault()?["captions"]?.FirstOrDefault();
            var description = imageCaption?["text"]?.ToString();
            var confidence = double.Parse(imageCaption?["confidence"]?.ToString().Replace(",", ".") ?? "0", CultureInfo.InvariantCulture);

            responseRecord.Data.Add("description", description);
            responseRecord.Data.Add("confidence", confidence);

            return new OkObjectResult(response);
        }
    }
}
