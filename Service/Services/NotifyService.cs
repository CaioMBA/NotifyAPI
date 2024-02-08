using AutoMapper;
using DataBaseConnections.OracleSqlDao;
using Domain.Interfaces;
using Domain.Models;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Mail;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Service.Services
{
    public class NotifyService : INotifyService
    {
        private CredentialDao _dao;
        private IMapper _mapper;

        public NotifyService(CredentialDao dao, IMapper mapper)
        {
            _dao = dao;
            _mapper = mapper;
        }

        public NotifyResponseModel DistributeNotifications(NotifyRequestModel obj)
        {
            NotifyResponseModel notifyResponseModel = new NotifyResponseModel();

            notifyResponseModel.MailResponses = new List<string?>();
            notifyResponseModel.PhoneResponses = new List<string?>();

            if (obj.Mails != null && obj.Mails.Count > 0)
            {
                notifyResponseModel.MailResponses = SendMail(obj.Mails);
            }
            if (obj.Phones != null && obj.Phones.Count > 0)
            {
                notifyResponseModel.PhoneResponses = SendPhoneMsg(obj.Phones);
            }

            return notifyResponseModel;

        }


        public List<string?>? SendMail(List<SendMailModel> ListModel)
        {
            var Credentials = _dao.GetCredentials();

            var UsingCredentiaal = (from c in Credentials
                                    where c.ACTIVE == 1
                                    & c.TYPE == "MAIL"
                                    orderby c.NOTIFYCREDENTIALID
                                    select c).FirstOrDefault();

            List<string>? Response = new List<string>();
            List<int> Ports = new List<int>() { 587, 465 };

            foreach (var obj in ListModel)
            {
                MailMessage MailMsg = new MailMessage()
                {
                    From = new MailAddress(UsingCredentiaal.CREDENTIAL, "Notify API"),
                    Subject = $"[NO-REPLY] {obj.Subject}",
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
                        using (SmtpClient MailClient = new SmtpClient(UsingCredentiaal.PROTOCOL, port))
                        {
                            MailClient.UseDefaultCredentials = false;
                            MailClient.EnableSsl = true;
                            MailClient.Credentials = new NetworkCredential(UsingCredentiaal.CREDENTIAL, UsingCredentiaal.PASSWORD);
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

        public List<string?>? SendPhoneMsg(List<SendPhoneMsgModel> ListModel)
        {
            var Credentials = _dao.GetCredentials();

            var UsingCredential = (from c in Credentials
                                   where c.ACTIVE == 1
                                   & c.TYPE == "TWILIO"
                                   orderby c.NOTIFYCREDENTIALID
                                   select c).FirstOrDefault();

            List<string> Response = new List<string>();

            foreach (var obj in ListModel)
            {
                Response = Response.Concat(SendSMSorWhatsApp(UsingCredential, obj)).ToList();

            }
            if (Response.Count == 0)
            {
                Response.Add("Função não implementada ainda!");
            }
            return Response;

        }

        public List<string?>? SendSMSorWhatsApp(CredentialsModel? Credential, SendPhoneMsgModel MsgModel)
        {
            List<string?> Responses = new List<string?>();

            var accountSid = Credential.CREDENTIAL;
            var authToken = Credential.PASSWORD;
            TwilioClient.Init(accountSid, authToken);

            foreach (var phone in MsgModel.Phones)
            {
                CreateMessageOptions MessageOption = new CreateMessageOptions("");

                switch (MsgModel.Type)
                {
                    case "WHATSAPP":
                        MessageOption = new CreateMessageOptions(new PhoneNumber($"whatsapp:{phone}"));
                        MessageOption.From = new PhoneNumber("whatsapp:+14155238886");
                        break;
                    case "SMS":
                        MessageOption = new CreateMessageOptions(new PhoneNumber(phone));
                        MessageOption.From = new PhoneNumber("+18159499432");
                        break;
                    default:
                        throw new Exception($"Type: |{MsgModel.Type}| Não implementado");
                }

                MessageOption.Body = $"{MsgModel.Name}, {MsgModel.Msg}";

                MessageResource message = MessageResource.Create(MessageOption);

                Responses.Add(message.Body);
            }

            return Responses;
        }

        /* TELEGRAM
        
        public string? SendTelegramMsg(string? token,List<string>? ChatIds,string? Msg)
        {
            var TelegramC = new TelegramBotClient(token);

            List<string> Response = new List<string>();

            foreach(var chat in ChatIds)
            {
                try
                {
                    TelegramC.SendTextMessageAsync(chatId: chat, text: Msg);
                }
                catch (Exception ex)
                {
                    Response.Add($"Erro ao tentar enviar msg ao chat: {ex.Message}");
                }
                
            }
            
            return null;
        }*/
    }
}
