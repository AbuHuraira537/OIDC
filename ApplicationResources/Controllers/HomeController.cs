using ApplicationResources.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationResources.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Route("api/other")]
        [Authorize(Policy  ="Other")]
        public string GetOtherRoleData()
        {
            return "Other Role Data";
        }
        
        [Route("api/authorize")]
        [Authorize]
        public string GetAuthorizeData()
        {
            return "Authorize Data";
        }
        [Route("api/admin")]
        [Authorize(Policy = "Admin")]
        public string GetAdminData()
        {
            return "Admin Role Data";
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
