using BLL.Service.Interface;
using BLL.Util;
using DAL.Models;
using DAL.Repository;
using DAL.Repository.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
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
    

        private static HubConnection? _connection;
        private static bool _connected = false;

        public AccountService(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;

            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7082/AccountSignalRChanel") 
                .WithAutomaticReconnect()
                .Build();

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

        public async Task<bool> BanAccountAsync(int accountId)
        {
            var account = await _accountRepo.GetAccountByIdAsync(accountId);
            if (account == null)
                return false;

            // Prevent banning admin accounts (RoleId = 1)
            if (account.RoleId == 1)
                return false;

            account.StatusId = 2; // 2 = banned
            await _accountRepo.UpdateAccountAsync(account);
            return true;
        }

        public async Task<bool> UnbanAccountAsync(int accountId)
        {
            var account = await _accountRepo.GetAccountByIdAsync(accountId);
            if (account == null)
                return false;

            account.StatusId = 1; // 1 = active
            await _accountRepo.UpdateAccountAsync(account);
            return true;
        }


        public async Task NotifyBanAccountAsync()
        {
            if (!_connected || _connection.State != HubConnectionState.Connected)
            {
                await _connection.StartAsync();
                _connected = true;
            }

            await _connection.InvokeAsync("SendAllLoadBanAccount");
        }


    }
}
