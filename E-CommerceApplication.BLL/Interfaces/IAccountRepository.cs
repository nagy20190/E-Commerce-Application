using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.BLL.Interfaces
{
    public interface IAccountRepository : IBaseRepository<User>
    {
        int emailCount(string email);
        public Task<User> GetUserByEmail(string email);


    }
}
