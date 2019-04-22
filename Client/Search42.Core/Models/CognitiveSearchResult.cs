using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Search42.Core.Models
{
    public class CognitiveSearchResult
    {
        [JsonProperty("metadata_storage_path")]
        public string Key { get; set; }

        [JsonProperty("imageTags")]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty("blob_uri")]
        public string Uri { get; set; }

        [JsonProperty("imageCaption")]
        public IEnumerable<string> RawImageCaptions { get; set; }

        private IList<ImageCaption> imageCaptions;
        public IEnumerable<ImageCaption> ImageCaptions
        {
            get
            {
                if (imageCaptions == null && (RawImageCaptions?.Any() ?? false))
                {
                    foreach (var rawImageCaption in RawImageCaptions)
                    {
                        imageCaptions = JToken.Parse(rawImageCaption)["captions"].ToArray().Select(c => new ImageCaption
                        {
                            Text = c["text"].ToString(),
                            Confidence = Convert.ToDouble(c["confidence"].ToString())
                        }).ToList();
                    }
                }

                return imageCaptions;
            }
        }

        public string Description => ImageCaptions.FirstOrDefault()?.Text;
    }
}
