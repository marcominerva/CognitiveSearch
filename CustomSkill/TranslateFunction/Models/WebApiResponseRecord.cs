using System.Collections.Generic;

namespace TranslateFunction.Models
{
    public class WebApiResponseRecord
    {
        public string RecordId { get; set; }

        public Dictionary<string, object> Data { get; set; }

        public List<WebApiResponseError> Errors { get; set; }

        public List<WebApiResponseWarning> Warnings { get; set; }
    }
}
