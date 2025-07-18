using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class AccountRepo : IAccountRepo
    {
        private readonly DemoContext _demoContext;

        public AccountRepo(DemoContext demoContext)
        {
            _demoContext = demoContext;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _demoContext.Accounts
                .Include(a => a.Role)
                .Include(a => a.Status)
                .ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int accountId)
        {
            return await _demoContext.Accounts
                .Include(a => a.Role)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(c => c.Id == accountId);
        }
        public async Task<Account> GetAccountByUserNameAsync(string username)
        {
            return await _demoContext.Accounts.FirstOrDefaultAsync(c => c.Name == username);
        }
        public async Task<Account> GetAccountByEmailAndPasswordAsync(string email, string? password)
        {
            if(string.IsNullOrEmpty(password)){
                return await _demoContext.Accounts.FirstOrDefaultAsync(c => c.Email == email);
            }
            return await _demoContext.Accounts.FirstOrDefaultAsync(c => c.Email == email && c.Password == password);
        }

        public async Task AddAccountAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            await _demoContext.Accounts.AddAsync(account);
            await _demoContext.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var existingAccount = await _demoContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id || a.Email == account.Email);
            if (existingAccount == null)
                throw new InvalidOperationException($"Account with ID {account.Id} not found.");

            // Update only non-null properties
            if (account.Name != null)
                existingAccount.Name = account.Name;
            
            if (account.Password != null)
                existingAccount.Password = account.Password;
            
            if (account.Phone != null)
                existingAccount.Phone = account.Phone;
            
            if (account.Address != null)
                existingAccount.Address = account.Address;
            
            if (account.RoleId.HasValue)
                existingAccount.RoleId = account.RoleId;
            
            if (account.StatusId.HasValue)
                existingAccount.StatusId = account.StatusId;

            _demoContext.Accounts.Update(existingAccount);
            await _demoContext.SaveChangesAsync();
        }
    }
}
