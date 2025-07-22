using DAL.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MVC.Models.User;
using BLL.Service.Interface;
using DAL.Models;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IOrderService _orderService;

        public UserController(IAccountService accountService, IOrderService orderService)
        {
            _accountService = accountService; // ✅ assign to the private field
            _orderService = orderService;
        }

        public async Task<IActionResult> Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var account = await _accountService.GetAccountByUserNameAsync(username);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            var orders = await _orderService.GetOrdersByUserIdAsync(account.Id); // 👈 load orders

            var viewModel = new ProfileViewModel
            {
                Email = account.Email,
                Name = account.Name,
                Phone = account.Phone,
                Address = account.Address,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            // get username from session
            var accountId = HttpContext.Session.GetInt32("AccountIdSession");
            if (accountId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var account = await _accountService.GetAccountByIdAsync(accountId.Value);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            var viewModel = new ProfileViewModel
            {
                Email = account.Email,
                Name = account.Name,
                Phone = account.Phone,
                Address = account.Address
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = new Account
            {
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address
            };

            await _accountService.UpdateAccountAsync(account);
            await _accountService.NotifyLoadAsync();

            TempData["Message"] = "Profile updated successfully.";
            return RedirectToAction("Profile", new { username = model.Name });
        }
    }
}



