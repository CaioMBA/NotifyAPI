using Domain.Interfaces;
using Domain.Models.Settings;
using System.Net.Http.Headers;
using System.Text;

namespace Service.Services
{
    public class ApiService : IApiService
    {
        public HttpResponseMessage APIRequest(ApiRequestModel ApiRequest)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                if (!String.IsNullOrEmpty(ApiRequest.Auth.Authorization))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiRequest.Auth.Type, ApiRequest.Auth.Authorization);
                }
                if (!String.IsNullOrEmpty(ApiRequest.Headers[0].Header))
                {
                    foreach (var header in ApiRequest.Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Header, header.Value);
                    }
                }
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));

                StringContent content = new StringContent(ApiRequest.Body, Encoding.UTF8, "application/json");

                httpClient.Timeout = TimeSpan.FromMinutes(3);

                HttpResponseMessage response = new HttpResponseMessage();

                switch (ApiRequest.TypeRequest)
                {
                    case "Post":
                        response = httpClient.PostAsync(ApiRequest.Url, content).Result;
                        break;
                    case "PUT":
                        response = httpClient.PutAsync(ApiRequest.Url, content).Result;
                        break;
                    case "GET":
                        var requestGet = new HttpRequestMessage(HttpMethod.Get, ApiRequest.Url)
                        {
                            Content = content
                        };
                        response = httpClient.SendAsync(requestGet).Result;
                        break;
                    case "DELETE":
                        var requestDelete = new HttpRequestMessage(HttpMethod.Delete, ApiRequest.Url)
                        {
                            Content = content
                        };
                        response = httpClient.SendAsync(requestDelete).Result;
                        break;
                    default:
                        throw new ArgumentException("Erro no switch Case de tipos de requisição!");
                }

                return response;
            }

        }
    }
}
