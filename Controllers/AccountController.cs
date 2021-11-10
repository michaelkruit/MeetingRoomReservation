using MeetingRooms.Exceptions;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            // Check if model is filled in correct
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            string token;

            try
            {
                // Log user in and generate token
                token = await _accountRepository.Login(loginViewModel);
            }
            catch (AccountException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(loginViewModel);
            }

            // Set token
            HttpContext.Session.SetString("Token", token);

            return RedirectToAction("Index", "MeetingRoom");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            // Register user
            try
            {
                await _accountRepository.Register(registerViewModel);
            }
            catch (AccountException e)
            {
                ModelState.AddModelError("CompanyName", e.Message);
                return View(registerViewModel);
            }
            // If user is registered go to login else try again
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Get token
            var token = HttpContext.Session.GetString("Token");
            // Remove user from cache
            _accountRepository.Logout(token);
            // Remove token from session
            HttpContext.Session.Remove("Token");

            return RedirectToAction(nameof(Login));
        }
    }
}
