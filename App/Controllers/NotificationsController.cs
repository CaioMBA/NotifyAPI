using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationsController : ControllerBase
    {
        private INotifyService _service;
        public NotificationsController(INotifyService service)
        {
            _service = service;
        }

        [HttpPost("/EnviarNotificacao")]
        public async Task<ActionResult> SendMail(
            [FromBody] NotifyRequestModel? Body)
        {

            var Response = _service.DistributeNotifications(Body);

            if (Response != null)
            {

                return StatusCode((int)HttpStatusCode.OK, Response);

            }
            else
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, Response);
            }
        }
    }
}
