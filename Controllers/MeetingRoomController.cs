using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    [Authorize]
    public class MeetingRoomController : Controller
    {
        public MeetingRoomController(IMeetingRoomIN)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
