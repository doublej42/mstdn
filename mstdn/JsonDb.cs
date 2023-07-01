using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace jsonDb
{
    public class JsonDb<T> where T : new()
    {
        [JsonIgnore]
        private readonly ILogger<JsonDb<T>> log;
        [JsonIgnore]
        private readonly IConfiguration config;
        private readonly string filename;

        public T Data { get; set; }

        public JsonDb(ILogger<JsonDb<T>> log, IConfiguration config) 
        {
            this.log = log;
            this.config = config;
            filename = config.GetValue<string>("jsonDb");
            if (!File.Exists(filename))
            {
                Data = new T();
            }
            else
            {
                var contents = File.ReadAllText(filename);
#pragma warning disable CS8601 // Possible null reference assignment.
                Data = JsonSerializer.Deserialize<T>(contents);
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        public void Save()
        {
            var redirectText = JsonSerializer.Serialize(Data,new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, redirectText);
        }
    }
}
