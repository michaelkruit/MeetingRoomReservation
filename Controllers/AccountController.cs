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
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var token = await _accountRepository.Login(loginViewModel);

            HttpContext.Session.SetString("Token", token);

            return RedirectToAction();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            var succes = await _accountRepository.Register(registerViewModel);
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
