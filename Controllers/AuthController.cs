using BusinessLayer.Services.Auth;
using DtoLayer.DTOs.AuthDtos;
using EntityLayer.Constants;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAcademy_CargoTrack.Models;

namespace MyAcademy_CargoTrack.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService,
            UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        // LOGIN GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
          
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginDto = new LoginDto
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            var result = await _authService.LoginAsync(loginDto);

            if (!result)
            {
                ModelState.AddModelError(string.Empty,
                    "E-posta adresi veya şifre hatalı.");

                return View(model);
            }

            // Kullanıcının bilgilerini al
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return RedirectToAction("Login");

            // Güvenli yerel yönlendirme
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Kullanıcının rollerini kontrol et
            if (await _userManager.IsInRoleAsync(user, RoleNames.Admin))
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (await _userManager.IsInRoleAsync(user, RoleNames.Manager))
                return RedirectToAction("Incoming", "Cargo", new { area = "Manager" });

            return RedirectToAction("Index", "User", new { area = "User" });
        }

        // REGISTER GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var registerDto = new RegisterDto
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password
            };

            var result = await _authService.RegisterAsync(registerDto);

            if (!result)
            {
                ModelState.AddModelError(string.Empty,
                    "Kayıt işlemi gerçekleştirilemedi. E-posta adresi kullanılıyor olabilir.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Kaydınız başarıyla oluşturuldu. Giriş yapabilirsiniz.";

            return RedirectToAction(nameof(Login));
        }

        // LOGOUT POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction(nameof(Login));
        }
    }
}

