using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface INotifyService
    {
        public NotifyResponseModel DistributeNotifications(NotifyRequestModel obj);
        public List<string>? SendMail(List<SendMailModel> ListModel);
        public List<string>? SendPhoneMsg(List<SendPhoneMsgModel> ListModel);
    }
}
