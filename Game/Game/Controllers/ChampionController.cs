using Microsoft.AspNetCore.Mvc;

namespace Game.Controllers
{
    public class ChampionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
