using DAL.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MVC.Models.User;
using BLL.Service.Interface;

namespace MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;

        public UserController(IAccountService accountService)
        {
            _accountService = accountService; // ✅ assign to the private field
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

            var viewModel = new ProfileViewModel
            {
                Email = account.Email,
                Name = account.Name,
                Phone = account.Phone,
                Address = account.Address
            };

            return View(viewModel);
        }
    }

}
