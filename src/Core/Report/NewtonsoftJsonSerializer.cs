using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetAssemblyInformer.Core.Report
{
    public class NewtonsoftJsonSerializer
    {
        public string Serialize<T>(T item)
        {
            var settings = GetDefaultSettings();
            var formatting = Formatting.Indented;
            string json = JsonConvert.SerializeObject(item, formatting, settings);
            return json;
        }

        private static JsonSerializerSettings GetDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Converters = new List<JsonConverter>
                                    {
                                        new Newtonsoft.Json.Converters.StringEnumConverter()
                                    }
            };
        }
    }

}