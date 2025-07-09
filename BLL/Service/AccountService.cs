using DAL.Models;
using BLL.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repository.Interface;
using DAL.Repository;
using BLL.Util;

namespace BLL.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;

        public AccountService(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        private void ValidateAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
        }
        public async Task AddAccountAsync(Account account)
        {
            // Business logic validation
            ValidateAccount(account);

            // Additional business rules can be added here
            // For example: Check for duplicate product names, validate price ranges, etc.
            account.Password = PasswordHashingHelper.HashPassword(account.Password);
            await _accountRepo.AddAccountAsync(account);
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepo.GetAllAccountsAsync();
        }

        public async Task<Account> GetAccountByEmailAndPasswordAsync(string email, string? password = null)
        {
            if (!string.IsNullOrEmpty(password))
            {
                password = PasswordHashingHelper.HashPassword(password);
            }
            return await _accountRepo.GetAccountByEmailAndPasswordAsync(email, password);
            
        }
        public async Task<Account> GetAccountByUserNameAsync(string username)
        {
            
            return await _accountRepo.GetAccountByUserNameAsync(username);

        }

        public async Task<Boolean> CheckDuplicateAccount(string email)
        {
            return await _accountRepo.GetAccountByEmailAndPasswordAsync(email, null) != null;
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {

            var account = await _accountRepo.GetAccountByIdAsync(id);
            if (account == null)
                throw new InvalidOperationException($"Account with ID {id} not found.");

            return account;
        }

        public async Task UpdateAccountAsync(Account account)
        {
            // Business logic validation
            ValidateAccount(account);

            // Additional business rules for updates can be added here

            await _accountRepo.UpdateAccountAsync(account);
        }
    }
}
