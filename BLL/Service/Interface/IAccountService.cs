using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task<Account> GetAccountByEmailAndPasswordAsync(string email, string? password = null);

        Task<Boolean> CheckDuplicateAccount(string email);

        Task AddAccountAsync(Account account);

        Task UpdateAccountAsync(Account account);
    }
}
