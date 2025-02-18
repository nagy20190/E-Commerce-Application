using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.DAL.Repositories
{
    public class ProductRepository :BaseRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;

        }
        public IQueryable<Product> query()
        {
            var query = _context.Products;
            return query;
        }


    }
}
