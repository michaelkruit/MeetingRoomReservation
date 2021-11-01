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

            // Log user in and generate token
            var token = await _accountRepository.Login(loginViewModel);

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
            var succes = await _accountRepository.Register(registerViewModel);
            // If user is registered go to login else try again
            return succes ? RedirectToAction(nameof(Login)) : View(registerViewModel);
        }

        [HttpPost]
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
