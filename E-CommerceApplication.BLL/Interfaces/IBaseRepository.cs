using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.BLL.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> GetAll();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRange(T entities);
        Task<T> Find(Expression<Func<T, bool>> match); // takes lambda expression and return the matched result
        Task<IEnumerable<T>> FindAllMatches(Expression<Func<T, bool>> match);
        Task<IEnumerable<T>> FindAllMatchesWithAnotherList(Expression<Func<T, bool>> match, string[] includes = null);
        // Add 
        Task<int> Count();

        IQueryable<T> query();
    }
}
