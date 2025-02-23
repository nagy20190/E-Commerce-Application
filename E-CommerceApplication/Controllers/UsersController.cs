using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Filters;
using E_CommerceApplication.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] // all the controller are for only admins
    public class UsersController : ControllerBase
    {
        // To Make Admin Read Users 
        private readonly IBaseRepository<User> _baseRepository;

        public UsersController(IBaseRepository<User> baseRepository)
        {
            _baseRepository = baseRepository;
            
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int? page)
        {
            if (page == null || page< 1)
            {
                page = 1;
            }
            int pageSize = 10;
            int totalPages = 0;

            decimal count = await _baseRepository.Count(); // count of users
            totalPages = (int)Math.Ceiling(count / pageSize);

            var users = await _baseRepository.GetAll();
            var usersOrderd = users.OrderByDescending(u => u.Id)
                .Skip((int)(page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (users == null)
            {
                return NotFound("no users in DB");
            }

            List<UserProfileDTO> userProfileDTOs = new List<UserProfileDTO>();
            foreach (var user in users)
            {
               var userProfileDto = new UserProfileDTO
                {
                   Id = user.Id,
                   firstName = user.FirstName,
                   lastName = user.LastName,
                   address = user.Address,
                   phone = user.Phone,
                   Role = user.Role,
                   CreatedAt = user.CreatedAt,
                };
                userProfileDTOs.Add(userProfileDto);
            }

            var response = new
            {
                TotalPages = totalPages,
                users = userProfileDTOs,
                page = page,
                pageSize = pageSize
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (id == 0 || id < 0) return BadRequest("not valid id ");

            var user = await _baseRepository.GetByIdAsync(id);
            if (user == null) { return NotFound($"no user with id {id}"); }

            var userDTO = new UserProfileDTO
            {
                Id = id,
                firstName = user.FirstName,
                lastName = user.LastName,
                address = user.Address,
                phone = user.Phone,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                email = user.Email,
                
            };
            return Ok(userDTO);
        }

        // search user with email or name
        [HttpGet("GetUserBySearch")]
        public IActionResult GetUserBySearch(string? search)
        {
            IQueryable<User> query = _baseRepository.query();
            if (search != null)
            {
                query = query.Where(p => p.FirstName.Contains(search) || p.Email.Contains(search)
                || p.LastName.Contains(search)); // search with mail or fname || lName
            }

            var users = query.ToList();

            List<UserProfileDTO> userProfileDTOs = new List<UserProfileDTO>();
            foreach (var user in users)
            {
                var userProfileDto = new UserProfileDTO
                {
                    Id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    address = user.Address,
                    phone = user.Phone,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    email = user.Email
                };
                userProfileDTOs.Add(userProfileDto);
            }
            return Ok(userProfileDTOs);
        }

        
    }
}
