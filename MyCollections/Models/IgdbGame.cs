using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyCollections.Models
{
    //https://app.quicktype.io/
    public partial class Igdb
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public long UpdatedAt { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("collection")]
        public long Collection { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("aggregated_rating")]
        public double AggregatedRating { get; set; }

        [JsonProperty("aggregated_rating_count")]
        public long AggregatedRatingCount { get; set; }

        [JsonProperty("total_rating")]
        public double TotalRating { get; set; }

        [JsonProperty("total_rating_count")]
        public long TotalRatingCount { get; set; }

        [JsonProperty("rating_count")]
        public long RatingCount { get; set; }

        [JsonProperty("games")]
        public long[] Games { get; set; }

        [JsonProperty("tags")]
        public long[] Tags { get; set; }

        [JsonProperty("developers")]
        public long[] Developers { get; set; }

        [JsonProperty("publishers")]
        public long[] Publishers { get; set; }

        [JsonProperty("game_engines")]
        public long[] GameEngines { get; set; }

        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("time_to_beat")]
        public TimeToBeat TimeToBeat { get; set; }

        [JsonProperty("player_perspectives")]
        public long[] PlayerPerspectives { get; set; }

        [JsonProperty("game_modes")]
        public long[] GameModes { get; set; }

        [JsonProperty("keywords")]
        public long[] Keywords { get; set; }

        [JsonProperty("themes")]
        public long[] Themes { get; set; }

        [JsonProperty("genres")]
        public long[] Genres { get; set; }

        [JsonProperty("standalone_expansions")]
        public long[] StandaloneExpansions { get; set; }

        [JsonProperty("first_release_date")]
        public long FirstReleaseDate { get; set; }

        [JsonProperty("pulse_count")]
        public long PulseCount { get; set; }

        [JsonProperty("platforms")]
        public long[] Platforms { get; set; }

        [JsonProperty("release_dates")]
        public ReleaseDate[] ReleaseDates { get; set; }

        [JsonProperty("screenshots")]
        public Cover[] Screenshots { get; set; }

        [JsonProperty("artworks")]
        public Cover[] Artworks { get; set; }

        [JsonProperty("videos")]
        public Video[] Videos { get; set; }

        [JsonProperty("cover")]
        public Cover Cover { get; set; }

        [JsonProperty("esrb")]
        public Esrb Esrb { get; set; }

        [JsonProperty("pegi")]
        public Pegi Pegi { get; set; }

        [JsonProperty("websites")]
        public Website[] Websites { get; set; }

        [JsonProperty("external")]
        public External External { get; set; }

        [JsonProperty("multiplayer_modes")]
        public MultiplayerMode[] MultiplayerModes { get; set; }
    }

    public partial class Cover
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("cloudinary_id")]
        public string CloudinaryId { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }

    public partial class Esrb
    {
        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("rating")]
        public long Rating { get; set; }
    }

    public partial class External
    {
        [JsonProperty("steam")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Steam { get; set; }
    }

    public partial class MultiplayerMode
    {
        [JsonProperty("platform")]
        public long Platform { get; set; }

        [JsonProperty("offlinecoop")]
        public bool Offlinecoop { get; set; }

        [JsonProperty("onlinecoop")]
        public bool Onlinecoop { get; set; }

        [JsonProperty("lancoop")]
        public bool Lancoop { get; set; }

        [JsonProperty("campaigncoop")]
        public bool Campaigncoop { get; set; }

        [JsonProperty("splitscreenonline")]
        public bool Splitscreenonline { get; set; }

        [JsonProperty("splitscreen")]
        public bool Splitscreen { get; set; }

        [JsonProperty("dropin")]
        public bool Dropin { get; set; }

        [JsonProperty("offlinecoopmax")]
        public long Offlinecoopmax { get; set; }

        [JsonProperty("onlinecoopmax")]
        public long Onlinecoopmax { get; set; }

        [JsonProperty("onlinemax")]
        public long Onlinemax { get; set; }

        [JsonProperty("offlinemax")]
        public long Offlinemax { get; set; }
    }

    public partial class Pegi
    {
        [JsonProperty("rating")]
        public long Rating { get; set; }
    }

    public partial class ReleaseDate
    {
        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("platform")]
        public long Platform { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("region", NullValueHandling = NullValueHandling.Ignore)]
        public long? Region { get; set; }

        [JsonProperty("human")]
        public string Human { get; set; }

        [JsonProperty("y")]
        public long Y { get; set; }

        [JsonProperty("m")]
        public long M { get; set; }
    }

    public partial class TimeToBeat
    {
        [JsonProperty("hastly")]
        public long Hastly { get; set; }

        [JsonProperty("normally")]
        public long Normally { get; set; }

        [JsonProperty("completely")]
        public long Completely { get; set; }
    }

    public partial class Video
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }
    }

    public partial class Website
    {
        [JsonProperty("category")]
        public long Category { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class Igdb
    {
        public static Igdb[] FromJson(string json) => JsonConvert.DeserializeObject<Igdb[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Igdb[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
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

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
