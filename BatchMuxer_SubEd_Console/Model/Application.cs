using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatchMuxer_SubEd_Console.Model
{
    [JsonObject("application")]
    public class Application
    {
        [JsonProperty("mkvMergePath")]
        public string MkvMergePath { get; set; }

        [JsonProperty("autoCleanUp")]
        public bool AutoCleanUp { get;set;}
    }
}
