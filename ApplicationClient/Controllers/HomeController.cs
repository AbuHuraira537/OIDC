using ApplicationClient.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> Login()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var claims = User.Claims;
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
           
            return View(new LoginResponseViewModel() { AccessToken = accessToken,IdToken = idToken, RefreshToken = refreshToken });
        }
        [Authorize]
        public async Task<IActionResult> AuthData()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await GetResourceApiData(accessToken);
            return View(new ResponseViewModel() { Value = result });
        }
        public async Task<IActionResult> Authorize()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await AuthorizeData(accessToken);
            int code = (int)result.StatusCode;
            ViewBag.code = code;
            return View();
        }
        [Authorize]
        public async Task<IActionResult> GetRefresh()
        {
            await GetRefreshTokenAsync();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            return View(new LoginResponseViewModel() { AccessToken = accessToken, RefreshToken = refreshToken });

        }
        [Authorize]
        public async Task<IActionResult> GetAdminData()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await GetResourceApiData(accessToken,"admin");
            return View(new ResponseViewModel() { Value = result });

        }
        [Authorize]
        public async Task<IActionResult> GetOtherData()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await GetResourceApiData(accessToken,"other");
            return View(new ResponseViewModel() { Value = result });

        }
        public IActionResult Logout()
        {
           return SignOut("Cookie", "oidc");
            //return RedirectToAction(nameof(Index));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetResourceApiData(string accessToken,string endpoint="authorize")
        {
            var apiClient = _clientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);

            var result = await apiClient.GetAsync($"https://localhost:44324/api/{endpoint}");
            if (!result.IsSuccessStatusCode)
            {
                return result.StatusCode.ToString();
            }
            var content = await result.Content.ReadAsStringAsync();
            return content;
        }
        private async Task<HttpResponseMessage> AuthorizeData(string accessToken, string endpoint = "authorize")
        {
            var apiClient = _clientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);

            var result = await apiClient.GetAsync($"https://localhost:44324/api/{endpoint}");
            var content = await result.Content.ReadAsStringAsync();
            return result;
        }
        private async Task GetRefreshTokenAsync()
        {
            var serverClient = _clientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44390/");

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshTokenClient = _clientFactory.CreateClient();

            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "clientIdMVC",

                ClientSecret = "client_secret",
            });
            var authInfo = await HttpContext.AuthenticateAsync("Cookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);
        }
    }
}
