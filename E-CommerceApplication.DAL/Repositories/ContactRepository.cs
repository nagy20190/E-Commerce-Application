using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.DAL.Repositories
{
    public class ContactRepository:BaseRepository<Contact>, IContactRepository
    {
        private readonly ApplicationDbContext _context;
        public ContactRepository(ApplicationDbContext context):base(context) 
        {
            _context = context;
            
        }

        public async Task<List<Contact>> GetAllWithSubjects(int page = 0, int pageSize = 5)
        {
            var contacts = await _context.Contacts.Include(c => c.Subject)
                .OrderByDescending(c => c.Id)
                .Skip((int)(page - 1) * pageSize )
                .Take(pageSize)
                .ToListAsync();
            return contacts;
        }

        public async Task<Contact> GetContactByIdWithSubjects(int id)
        {
            var contact = await _context.Contacts.Include(c => c.Subject).FirstOrDefaultAsync(c => c.Id == id);
            return contact!;
        }
    }
}
