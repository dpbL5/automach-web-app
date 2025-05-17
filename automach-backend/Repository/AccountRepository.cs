using System.Net.NetworkInformation;
using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Models;

namespace automach_backend.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext context;

        public AccountRepository(DataContext context)
        {
            this.context = context;
        }

        public bool AccountExists(int id)
        {
            // Any() will return true if any element in the sequence satisfies the condition
            return context.Accounts.Any(a => a.Id == id);
        }

        // FirstOrDefault() will return the first element of a sequence or a default value if no element is found
        public Account GetAccount(int id)
        {
            return context.Accounts.Where(a => a.Id == id).FirstOrDefault();
        }

        public Account GetAccount(string username)
        {
            return context.Accounts.Where(a => a.Username == username).FirstOrDefault();
        }

        public Account GetAccount(string username, string password)
        {
            return context.Accounts.Where(a => a.Username == username && a.Password == password).FirstOrDefault();
        }

        public ICollection<Account> GetAccounts()
        {
            // Phai su dung ToList() de explicitly execute the query
            // Neu khong ICollection se kh biet kieu gi
            return context.Accounts.OrderBy(a => a.Id).ToList();
        }
    }   
}
