using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {

        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(object loginModel) 
        {
            // TODO: Login user

            // After login is success, redirect user to dashboard
            return RedirectToAction();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(object registerModel)
        {
            // TODO: Register new user

            // After register is success, redirect user to dashboard
            return RedirectToAction();
        }
    }
}
