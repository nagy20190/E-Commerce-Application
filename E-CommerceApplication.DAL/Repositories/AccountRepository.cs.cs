using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;

namespace E_CommerceApplication.DAL.Repositories
{
    public class AccountRepository : BaseRepository<User>, IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        public AccountRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
            
        }
        public int emailCount(string email)
        {
            int numberOfEmails = _context.Users.Count(u => u.Email == email);
            return numberOfEmails;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email)!;
        }
    }
}
