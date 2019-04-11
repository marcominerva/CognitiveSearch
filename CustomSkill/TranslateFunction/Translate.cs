using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TranslateFunction.Models;

namespace TranslateFunction
{
    // This function will simply translate messages sent to it.
    public static class Translate
    {
        private const string path = "https://api.cognitive.microsofttranslator.com/translate?api-version=3.0";

        // NOTE: Replace this example key with a valid subscription key.
        private const string key = "";

        [FunctionName("Translate")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req,
            ILogger log)
        {
            var requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            // Validation
            if (data?.values == null)
            {
                return new BadRequestObjectResult(" Could not find values array");
            }
            if (data?.values.HasValues == false || data?.values.First.HasValues == false)
            {
                // It could not find a record, then return empty values array.
                return new BadRequestObjectResult(" Could not find valid records in values array");
            }

            var recordId = data?.values?.First?.recordId?.Value as string;
            if (recordId == null)
            {
                return new BadRequestObjectResult("recordId cannot be null");
            }

            var originalText = data?.values?.First?.data?.text?.Value as string;
            var toLanguage = "en";

            var translatedText = await TranslateTextAsync(originalText, toLanguage);

            // Put together response.
            var responseRecord = new WebApiResponseRecord
            {
                Data = new Dictionary<string, object>(),
                RecordId = recordId
            };
            responseRecord.Data.Add("text", translatedText);

            var response = new WebApiEnricherResponse
            {
                Values = new List<WebApiResponseRecord>()
            };
            response.Values.Add(responseRecord);

            return new OkObjectResult(response);
        }


        /// <summary>
        /// Use Cognitive Service to translate text from one language to another.
        /// </summary>
        /// <param name="originalText">The text to translate.</param>
        /// <param name="toLanguage">The language you want to translate to.</param>
        /// <returns>Asynchronous task that returns the translated text. </returns>
        private static async Task<string> TranslateTextAsync(string originalText, string toLanguage)
        {
            var body = new object[] { new { Text = originalText } };
            var requestBody = JsonConvert.SerializeObject(body);

            var uri = $"{path}&to={toLanguage}";

            var result = string.Empty;

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(uri);
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", key);

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    dynamic data = JsonConvert.DeserializeObject(responseBody);
                    result = data?.First?.translations?.First?.text?.Value as string;
                }
            }

            return result;
        }
    }
}