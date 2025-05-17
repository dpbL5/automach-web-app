using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IAccountRepository
    {   
        // GET 
        ICollection<Account> GetAccounts();

        Account GetAccount(int id);

        Account GetAccount(string username);

        Account GetAccount(string username, string password);

        
        bool AccountExists(int id);
    }
}