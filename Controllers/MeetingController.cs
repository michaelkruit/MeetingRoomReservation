using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    public class MeetingController : Controller
    {
        public MeetingController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
