using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace CodeBase.Data
{
    public static class DataExtensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, CreateSettings());
        }

        public static T FromJson<T>(this string json)
        {
            try {
                return JsonConvert.DeserializeObject<T>(json, CreateSettings());
            } catch (Exception ex) {
                return default;
            }
        }

        private static JsonSerializerSettings CreateSettings()
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return settings;
        }
    }
}