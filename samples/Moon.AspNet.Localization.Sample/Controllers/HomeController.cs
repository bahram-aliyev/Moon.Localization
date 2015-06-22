using Microsoft.AspNet.Mvc;

namespace Moon.AspNet.Localization.Sample.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}