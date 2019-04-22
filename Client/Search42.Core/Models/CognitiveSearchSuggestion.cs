using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Search42.Core.Models
{
    public class CognitiveSearchSuggestion
    {
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty("captions")]
        public IEnumerable<ImageCaption> ImageCaptions { get; set; }

        public string Description => ImageCaptions.FirstOrDefault()?.Text;
    }
}
