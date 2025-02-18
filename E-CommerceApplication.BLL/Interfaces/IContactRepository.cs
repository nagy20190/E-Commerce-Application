using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;

namespace E_CommerceApplication.BLL.Interfaces
{
    public interface IContactRepository:IBaseRepository<Contact>
    {
        Task<List<Contact>> GetAllWithSubjects(int page = 0, int pageSize = 5);
        Task<Contact> GetContactByIdWithSubjects(int id);

    }
}
