using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeBase.Utils
{
    public static class JsonUtils
    {
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, CreateSettings());
        }
        
        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, CreateSettings());
        }

        [NotNull]
        private static JsonSerializerSettings CreateSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return settings;
        }
    }
}