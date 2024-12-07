using Domain.Common;
using Domain.Exceptions;
using Domain.SeedWork.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class TestController() : BaseController
    {
        [HttpGet("405")]
        [EndpointSummary("Default exception")]
        [EndpointDescription("Returns not allowed exception")]
        public IActionResult GetNotAllowed()
        {
            throw new NotAllowedException();
        }

        [HttpGet("400")]
        [EndpointSummary("Default bad request")]
        [EndpointDescription("Returns bad request")]
        public IActionResult GetBadRequest()
        {
            NotificationsWrapper.AddNotification("Test error");
            throw new NotificationException();
        }

        [HttpGet("200")]
        [EndpointSummary("Default OK")]
        [EndpointDescription("Returns OK")]
        public IActionResult Get()
        {
            return Ok(new GenericResponse<object>(null));
        }
    }
}
