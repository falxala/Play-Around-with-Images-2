using Newtonsoft.Json;
using System.Collections.Generic;

namespace PlayAroundwithImages2
{
    [JsonObject]
    internal class Content
    {
        public string id { get; set; }
        public string tag_name { get; set; }
        public object[] assets { get; set; }

    }
}