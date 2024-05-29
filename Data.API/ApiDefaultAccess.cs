using Domain.Models.GeneralSettings;
using System.Text;

namespace Data.API
{
    public class ApiDefaultAccess
    {
        public HttpResponseMessage ApiRequest(ApiRequestModel ApiRequest)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpMethod Method = HttpMethod.Get;
                switch (ApiRequest.TypeRequest)
                {
                    case "GET":
                        Method = HttpMethod.Get;
                        break;
                    case "POST":
                        Method = HttpMethod.Post;
                        break;
                    case "PUT":
                        Method = HttpMethod.Put;
                        break;
                    case "DELETE":
                        Method = HttpMethod.Delete;
                        break;
                    case "HEAD":
                        Method = HttpMethod.Head;
                        break;
                    default:
                        new Exception("Nenhum Método Aceito");
                        break;
                }
                var request = new HttpRequestMessage(Method, ApiRequest.Url);



                if (ApiRequest.TimeOut != null)
                {
                    httpClient.Timeout = TimeSpan.FromSeconds((double)ApiRequest.TimeOut);
                }
                if (ApiRequest.Auth != null)
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiRequest.Auth.Type, ApiRequest.Auth.Authorization);
                }
                foreach (var header in ApiRequest.Headers!)
                {
                    request.Headers.Add(header.Header, header.Value);
                }
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                if (!String.IsNullOrEmpty(ApiRequest.Body))
                {
                    request.Content = new StringContent(ApiRequest.Body, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage? response = httpClient.SendAsync(request).Result;


                return response;
            }

        }
    }
}
