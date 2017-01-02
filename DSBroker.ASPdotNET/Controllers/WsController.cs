using Microsoft.AspNetCore.Mvc;

namespace DSBroker.ASPdotNET.Controllers
{
    [Route("ws")]
    public class WsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
