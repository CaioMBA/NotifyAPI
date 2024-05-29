using AutoMapper;
using Data.API;
using Data.DataBase.SqlServerDAO;
using Domain.Interfaces;
using Domain.Models;
using Domain.Utils;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Service.Services
{
    public class NotifyService : INotifyService
    {
        private ApiDefaultAccess _api;
        private CredentialDAO _dao;
        private IMapper _mapper;
        private Utils _utils;
        public NotifyService(ApiDefaultAccess api, CredentialDAO dao, IMapper mapper, Utils utils)
        {
            _api = api;
            _dao = dao;
            _mapper = mapper;
            _utils = utils;
        }


        #region Raw
        private List<string> SendMail(List<SendMailModel> ListModel)
        {
            var Credentials = _dao.GetCredentials();

            if (Credentials == null || Credentials.Count == 0)
            {
                throw new Exception("Não foi possível encontrar nenhuma credencial");
            }

            CredentialInfoModel? UsingCredential = (from c in Credentials
                                                    where c.Active == true
                                                    & c.Type == "MAIL"
                                                    orderby c.CredentialID
                                                    select c).FirstOrDefault();
            if (UsingCredential == null)
            {
                throw new Exception("Não foi possível encontrar credenciais de e-mail ativas");
            }

            List<string>? Response = new List<string>();
            List<int> Ports = new List<int>() { 587, 465 };

            foreach (var obj in ListModel)
            {
                MailMessage MailMsg = new MailMessage()
                {
                    From = new MailAddress(UsingCredential.Credential, "Notify API"),
                    Subject = $"[ NO-REPLY ] {obj.Subject}",
                    Body = obj.Msg,
                    Priority = MailPriority.Normal
                };
                foreach (var to in obj.MailDestinations)
                {
                    MailMsg.To.Add(new MailAddress(to));
                }

                foreach (var port in Ports)
                {
                    try
                    {
                        using (SmtpClient MailClient = new SmtpClient(UsingCredential.Protocol, port))
                        {
                            MailClient.UseDefaultCredentials = false;
                            MailClient.EnableSsl = true;
                            MailClient.Credentials = new NetworkCredential(UsingCredential.Credential, UsingCredential.SecretKey);
                            MailClient.Send(MailMsg);
                        }
                        Response.Add("E-mail enviado com sucesso");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Response.Add($"O envio do e-mail não funcionou, erro: {ex.Message}");
                    }
                }
            }

            return Response;
        }

        private List<string> SendPhoneMsg(List<SendPhoneMsgModel> ListModel)
        {
            var Credentials = _dao.GetCredentials();

            if (Credentials == null || Credentials.Count == 0)
            {
                throw new Exception("Não foi possível encontrar nenhuma credencial");
            }

            var UsingCredential = (from c in Credentials
                                   where c.Active == true
                                   & c.Type == "TWILIO"
                                   orderby c.CredentialID
                                   select c).FirstOrDefault();
            if (UsingCredential == null)
            {
                throw new Exception("Não foi possível encontrar credenciais de e-mail ativas");
            }

            TwilioClient.Init(UsingCredential.Credential, UsingCredential.SecretKey);

            List<string> Responses = new List<string>();

            foreach (var obj in ListModel)
            {
                foreach (string? phone in obj.Phones)
                {
                    string FromNumber = "+18159499432";
                    string ToNumber = phone;
                    if (obj.Type == "WHATSAPP")
                    {
                        FromNumber = "whatsapp:+14155238886";
                        ToNumber = $"whatsapp:{phone}";
                    }

                    CreateMessageOptions options = new CreateMessageOptions(new PhoneNumber(ToNumber))
                    {
                        From = new PhoneNumber(FromNumber),
                        Body = obj.Msg
                    };
                    MessageResource Msg = MessageResource.Create(options);
                    Responses.Add(Msg.Body);
                }
            }

            return Responses;
        }
        #endregion

        public NotifyResponseModel SendNotification(NotifyRequestModel Request)
        {
            NotifyResponseModel Response = new NotifyResponseModel();

            if (Request.Mails != null && Request.Mails.Count > 0)
            {
                try
                {
                    Response.MailResponses = SendMail(Request.Mails);
                }
                catch (Exception ex)
                {
                    Response.MailResponses = new List<string>() { ex.Message };
                }
            }

            if (Request.Phones != null && Request.Phones.Count > 0)
            {
                try
                {
                    Response.PhoneResponses = SendPhoneMsg(Request.Phones);
                }
                catch (Exception ex)
                {
                    Response.PhoneResponses = new List<string>() { ex.Message };
                }
            }

            return Response;
        }

    }
}
