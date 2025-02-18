using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;

namespace E_CommerceApplication.BLL.Interfaces
{
    public interface IProductRepository:IBaseRepository<Product>
    {
        IQueryable<Product> query();

    }
}
