using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Search42.Core.Models
{
    public class CognitiveSearchResult
    {
        [JsonProperty("metadata_storage_path")]
        public string Key { get; set; }

        [JsonProperty("imageTags")]
        public IEnumerable<string> Tags { get; set; }

        private string uri;
        public string Uri
        {
            get
            {
                if (uri == null)
                {
                    var base64Uri = Key.Substring(0, Key.Length - 1).Replace(" ", "+");
                    if (base64Uri.Length % 4 > 0)
                    {
                        base64Uri = base64Uri.PadRight(base64Uri.Length + 4 - base64Uri.Length % 4, '=');
                    }

                    var byteArray = Convert.FromBase64String(base64Uri);
                    uri = Encoding.UTF8.GetString(byteArray);
                }

                return uri;
            }
        }

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
