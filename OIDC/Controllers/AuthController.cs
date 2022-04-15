using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OIDC.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, 
            IIdentityServerInteractionService interactionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interactionService = interactionService;
        }
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutContext = await _interactionService.GetLogoutContextAsync(logoutId);
            if (string.IsNullOrWhiteSpace(logoutContext.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index");
            }
            return Redirect(logoutContext.PostLogoutRedirectUri);
        }
        [HttpGet]
        public IActionResult Login(string returnurl)
        {
            return View(new LoginViewModel { ReturnUrl = returnurl});
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            var context = HttpContext;
            var contextResponse  = HttpContext.Response;
            if (ModelState.IsValid)
            {
                var result =await _signInManager.PasswordSignInAsync(vm.UserName,vm.Password,false,false);
                if (result.Succeeded)
                {
                   return Redirect(vm.ReturnUrl);
                }
            }
            return View(vm);
        }
        [HttpGet]
        public IActionResult Register(string returnurl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnurl});
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser(vm.UserName);
                var result =await _userManager.CreateAsync(user,vm.Password);
                if (result.Succeeded)
                {
                  await  _signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, false);
                   return Redirect(vm.ReturnUrl);
                }
            }
            return View(vm);
        }
    }
}
