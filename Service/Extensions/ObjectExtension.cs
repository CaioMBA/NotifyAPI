using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using System.Text.Json;

namespace Service.Extensions
{
    public static class ObjectExtension
    {
        public static string To_Json(this Object obj)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                //Formatting = Formatting.Indented,
                StringEscapeHandling = StringEscapeHandling.Default,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, jsonSettings);
        }

        public static string To_UTF8Json(this Object obj)
        {
            var jsonBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            return Encoding.UTF8.GetString(jsonBytes);
        }

        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        public static string BsonToJson(this BsonDocument bson, DateTimeZoneHandling timeZoneHandling = DateTimeZoneHandling.Utc)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(stream))
                {
                    BsonSerializer.Serialize(writer, typeof(BsonDocument), bson);
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new Newtonsoft.Json.Bson.BsonReader(stream))
                {
                    var sb = new StringBuilder();
                    var jsonWriterSettings = new JsonWriterSettings
                    {
                        OutputMode = JsonOutputMode.Strict
                    };

                    using (var jWriter = new JsonTextWriter(new StringWriter(sb)))
                    {
                        jWriter.DateTimeZoneHandling = timeZoneHandling;
                        jWriter.WriteToken(reader, true);
                    }

                    return sb.ToString();
                }
            }
        }
    }
}
