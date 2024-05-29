using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using System.Text.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace Service.Extensions
{
    public static class StringExtension
    {
        public static string FirstToUpper(this String str)
        {
            string Primeira = str.Substring(0, 1);


            string Segunda = str.Substring(1);

            return Primeira.ToUpper() + Segunda;

        }

        public static bool ToInteger(this String str, out int resultado)
        {
            bool sucesso = Int32.TryParse(str, out resultado);
            return sucesso;

        }

        public static string LimitStringLength(this String str, int Limit)
        {
            if (Limit > str.Length)
                return str;

            return str.Substring(0, Limit);

        }

        public static string ToSHA256(this String str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // Convert byte to hexadecimal string
                }
                return builder.ToString();
            }
        }

        public static string ToBase64(this String str)
        {
            try
            {
                byte[] textoAsBytes = UTF8Encoding.UTF8.GetBytes(str);
                string resultado = Convert.ToBase64String(textoAsBytes);
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Base64ToString(this String str)
        {
            try
            {
                byte[] dadosAsBytes = Convert.FromBase64String(str);
                string resultado = UTF8Encoding.UTF8.GetString(dadosAsBytes);
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static T JsonStringToObject<T>(this String str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static object JsonStringToObject(this String str)
        {
            dynamic stuff = JObject.Parse(str);
            return stuff;
            //var data = JsonConvert.DeserializeObject<IEnumerable<dynamic>>(str);
        }

        public static T BsonStringToObject<T>(this String BsonString)
        {
            return ToDynamic(BsonSerializer.Deserialize<BsonDocument>(BsonString));
        }

        #region BsonConversion
        private static dynamic ToDynamic(BsonDocument bsonDocument)
        {
            dynamic dynamicObject = new System.Dynamic.ExpandoObject();

            foreach (var element in bsonDocument.Elements)
            {
                ((IDictionary<string, object?>)dynamicObject)[element.Name] = ConvertBsonValue(element.Value);
            }
            return dynamicObject;
        }

        private static object? ConvertBsonValue(BsonValue bsonValue)
        {
            switch (bsonValue.BsonType)
            {
                case BsonType.ObjectId:
                    return bsonValue.AsObjectId.ToString();
                case BsonType.String:
                    return bsonValue.ToString();
                case BsonType.Int32:
                    return bsonValue.ToInt32();
                case BsonType.Int64:
                    return bsonValue.ToInt64();
                case BsonType.Double:
                    return bsonValue.ToDouble();
                case BsonType.Boolean:
                    return bsonValue.ToBoolean();
                case BsonType.Array:
                    List<dynamic> Return = new List<dynamic>();
                    foreach (BsonDocument item in bsonValue.AsBsonArray.ToList())
                    {
                        if (item != null)
                        {
                            string json = item.BsonToJson();
                            dynamic obj = json.BsonStringToObject<dynamic>();
                            try
                            {
                                obj._id = item["_id"].ToString();
                            }
                            catch { }
                            Return.Add(obj);
                        }
                    }
                    return Return;
                case BsonType.Null:
                    return null;
                case BsonType.Undefined:
                    return null;
                default:
                    return bsonValue;
            }
        }
        #endregion

        public static bool IsBase64String(this string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }
    }
}
