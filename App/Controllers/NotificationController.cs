using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private INotifyService _service;
        public NotificationController(INotifyService service)
        {
            _service = service;
        }

        [HttpPost("Send")]
        public async Task<ActionResult> Post(
            [FromBody] NotifyRequestModel Body
            )
        {
            return StatusCode((int)HttpStatusCode.OK, _service.SendNotification(Body));
        }
    }
}
