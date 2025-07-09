using BLL.Service.Interface;
using BLL.Util;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Threading.Tasks;
using DAL.Models;
using System.Text.Json;
using System.Collections.Generic;
using MVC.Models.Auth;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAccountService _accountService;

        public AuthController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> CheckIsLogin(){
            if(Request.Cookies.TryGetValue("CookieUser", out var accountJson)){
                var accountRaw = JsonSerializer.Deserialize<Account>(accountJson);
                if(accountRaw != null){
                    var account = await _accountService.GetAccountByEmailAndPasswordAsync(accountRaw.Email, null);
                    if(accountRaw.RoleId == 2){
                        SetSession(account);
                        
                        return RedirectToAction("Index", "Home");
                    }else{
                        SetSession(account);
                        return Redirect("/razor/Index");
                    }
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        public void SetCookie(Account account){
            var accountJson = new {
                    Email = account.Email,
                    RoleId = account.RoleId
                };
                Response.Cookies.Append("CookieUser", JsonSerializer.Serialize(accountJson), new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(3),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
        }

        public void SetSession(Account account){
            HttpContext.Session.SetInt32("AccountIdSession", account.Id);
            HttpContext.Session.SetString("RoleIdSession", account.RoleId.ToString());
            HttpContext.Session.SetString("AccountName", account.Name ?? "Guest");
            HttpContext.Session.SetString("AccountEmail", account.Email);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = await _accountService.GetAccountByEmailAndPasswordAsync(model.Email, model.Password);
            
            if (account == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // TODO: Add authentication logic here (e.g., using ASP.NET Core Identity or custom authentication)
            // For now, we'll just redirect to home page
            SetCookie(account);
            SetSession(account);

            if (account.RoleId == 2)//Customer
            {
               

                return RedirectToAction("Index", "Home");
            }else{
                //Admin
                return Redirect("razor/Index");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if email already exists
            if (await _accountService.CheckDuplicateAccount(model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(model);
            }

            // Create new account
            var account = new Account
            {
                Email = model.Email,
                Password = model.Password, // Note: In a real application, you should hash the password
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                RoleId = 2, // Assuming 2 is the default role ID for regular users
                StatusId = 1 // Assuming 1 is the active status ID
            };

            try
            {
                // Convert account to JSON string
                var accountJson = JsonSerializer.Serialize(account);

                // Save to cookie (expires in 1 day)
                Response.Cookies.Append("PendingAccount", accountJson, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
                EmailHelper.SendRegisterEmail(account.Email);
                // Redirect to login page after successful registration
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while registering. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string email)
        {
            if (!Request.Cookies.TryGetValue("PendingAccount", out var accountJson))
            {
                return BadRequest("No registration data found.");
            }

            var account = JsonSerializer.Deserialize<Account>(accountJson);

            if (account == null || account.Email != email)
            {
                return BadRequest("Invalid or expired verification.");
            }

            // Save to database
            await _accountService.AddAccountAsync(account);

            // Clear cookie
            Response.Cookies.Delete("PendingAccount");

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var account = await _accountService.GetAccountByEmailAndPasswordAsync(model.Email);
            if (account == null)
            {
                ModelState.AddModelError("", "Email does not exist.");
                return View(model);
            }

            // Generate OTP
            var otp = new Random().Next(1000, 9999).ToString();
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("ResetEmail", model.Email);

            // Send OTP via email
            await EmailHelper.SendForgetPasswordEmail(model.Email, otp);

            // Pass email to VerifyOtp view
            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }

        [HttpGet]
        public IActionResult VerifyOtp(string email)
        {
            var model = new ResetPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sessionOtp = HttpContext.Session.GetString("ResetOTP");
            var sessionEmail = HttpContext.Session.GetString("ResetEmail");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(sessionEmail))
            {
                ModelState.AddModelError("", "OTP session expired. Please request a new OTP.");
                return View(model);
            }

            if (model.OTP != sessionOtp)
            {
                ModelState.AddModelError("", "Invalid OTP.");
                return View(model);
            }

            // reset password
            model.NewPassword = PasswordHashingHelper.HashPassword(model.NewPassword);
            
            await _accountService.UpdateAccountAsync(new Account { Email = model.Email, Password = model.NewPassword });
            HttpContext.Session.Remove("ResetOTP");
            HttpContext.Session.Remove("ResetEmail");

            TempData["Message"] = "Password reset successful. Please login with your new password.";
            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            // Get email from session
            var email = HttpContext.Session.GetString("AccountEmail");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var account = await _accountService.GetAccountByEmailAndPasswordAsync(email);
            if (account == null)
            {
                TempData["Message"] = "Account not found.";
                return RedirectToAction("Login");
            }

            // Generate OTP
            var otp = new Random().Next(1000, 9999).ToString();
            HttpContext.Session.SetString("ResetOTP", otp);
            HttpContext.Session.SetString("ResetEmail", email);

            // Send OTP via email
            await EmailHelper.SendChangePasswordEmail(email, otp);

            // Redirect to VerifyOtp
            return RedirectToAction("VerifyOtp", new { email = email });
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("CookieUser");
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
