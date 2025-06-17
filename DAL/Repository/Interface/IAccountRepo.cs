using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IAccountRepo
    {
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task<Account> GetAccountByEmailAndPasswordAsync(string email, string? password);

        Task AddAccountAsync(Account account);

        Task UpdateAccountAsync(Account account);
    }
}
