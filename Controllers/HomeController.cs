using MeetingRooms.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MeetingRooms.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errorMessage)
        {
            errorMessage ??= "Something went wrong, please try again or contact your admin";
            return View(new ErrorViewModel { ErrorMessage = errorMessage });
        }
    }
}
