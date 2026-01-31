using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Users;
using MITCRMS.Models.Entities;
using System.Security.Claims;

namespace MITCRMS.Controllers
{
    public class UserController : Controller
    {
        private IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult StaffDashboard()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            var loginResponse = await _userServices.LoginAsync(model, CancellationToken.None);
            var checkRole = "";
            if (loginResponse.Status)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginResponse.Data.FirstName),
                    new Claim(ClaimTypes.GivenName, loginResponse.Data.FullName),
                    new Claim(ClaimTypes.Email, loginResponse.Data.Email),
                    new Claim(ClaimTypes.NameIdentifier, loginResponse.Data.UserId.ToString()),
                };
                 
                foreach (var role in loginResponse.Data.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                    checkRole = role.Name;
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authenticationProperties = new AuthenticationProperties();
                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal, authenticationProperties);
                if (checkRole == "Tutor")
                {
                    return RedirectToAction("StaffDashboard", "User");
                }
                else if (checkRole=="Bursar")
                { 
                    return RedirectToAction("StaffDashboard", "User");
                }
                else if (checkRole=="Admin")
                {
                    return RedirectToAction("StaffDashboard"," User");
                }
                else if (checkRole == "Hod")
                {
                    return RedirectToAction("StaffDashboard", "User"); 
                }
                return RedirectToAction("CreateDepartment", "Department");

            }
            else
            {
                ViewBag.ErrorMessage = loginResponse.Message;
                return View(model);
            }

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserId");

            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";
            return RedirectToAction("Login", "User");
        } 
    }


}
