using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Filters;
using E_CommerceApplication.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IBaseRepository<User> _baseRepository;
        public UsersController(IBaseRepository<User> baseRepository)
        {
            _baseRepository = baseRepository;
            
        }


      
        
    }
}
