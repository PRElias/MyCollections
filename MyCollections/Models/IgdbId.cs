using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyCollections
{
    public partial class IgdbIds
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class IgdbIds
    {
        public static IgdbIds[] FromJson(string json) => JsonConvert.DeserializeObject<IgdbIds[]>(json, MyCollections.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this IgdbIds[] self) => JsonConvert.SerializeObject(self, MyCollections.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
