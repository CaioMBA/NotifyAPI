using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
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
            if(Limit > str.Length)
                return str;

            return str.Substring(0, Limit);

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
    }
}
