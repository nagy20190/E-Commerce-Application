using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBaseRepository<User> _baseRepository;
        private readonly IBaseRepository<PasswordReset> _passwordResetRepository;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _userRepository;
        private readonly EmailSender _emailSender;

        public AccountController(IConfiguration configuration,
            IBaseRepository<User> baseRepository,
            IAccountRepository userRepository, IBaseRepository<PasswordReset> passwordResetRepository, EmailSender emailSender)
        {
            _configuration = configuration;
            _baseRepository = baseRepository;
            _userRepository = userRepository;
            _passwordResetRepository = passwordResetRepository;
            _emailSender = emailSender;
        }
        private string CreateJwToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                // the user id
                new Claim("id", "" + user.Id /* the "" because the Claim object takes two stirng paramts*/),
                new Claim("role", "" + user.Role /* we add role here to do the auth with auth middlware*/),

            };

            string scrtKey = _configuration["JWTSettings:Key"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(scrtKey));

            var credintials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTSettings:Issuer"],
                audience: _configuration["JWTSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(2), // Token Valid for 2 Days,
                signingCredentials: credintials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        // Register users
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            // check if the email is already use or not 
            var emailCount = _userRepository.emailCount(userDTO.email); // connect to DB and return number of emails mathes the email
            if (emailCount > 0)
            {
                ModelState.AddModelError("Email", "This email is already used 1");
                return BadRequest(ModelState);
            }

            // encrypt the recived password
            var passwordHasher = new PasswordHasher<User>();
            var ecryptedPassword = passwordHasher.HashPassword(new User(), userDTO.password);

            // create a new account 
            User user = new User()
            {
                FirstName = userDTO.firstName,
                LastName = userDTO.lastName,
                Email = userDTO.email,
                Password = ecryptedPassword,
                Address = userDTO.address,
                Role = "client",
                Phone = userDTO.phone??"",
                CreatedAt = DateTime.Now,
                
            };

            try
            {
                await _baseRepository.AddAsync(user);
            }
            catch (Exception)
            {
                return StatusCode(500, "an error occured while Register");
            }
            // create the token
            var jwt = CreateJwToken(user);

            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                firstName = userDTO.firstName,
                lastName = userDTO.lastName,
                email = userDTO.email,
                address = userDTO.address,
                Role = "client",
                phone = userDTO.phone ?? "",
                CreatedAt = DateTime.Now,

            };

            var response = new
            {
                Jwt = jwt,
                userProfile = userProfileDTO

            };

            return Ok(response);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            
            if (user == null)
            {
                ModelState.AddModelError("Error", "Email or password not valid");
                return BadRequest(ModelState);
            }

            // verify password
            var passwordHasher = new PasswordHasher<User>();

            var result = passwordHasher.VerifyHashedPassword(new User(), user.Password, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "wrong password");
                return BadRequest(ModelState);
            }

            var jwt = CreateJwToken(user);

            UserProfileDTO userProfileDTO = new UserProfileDTO()
            {
                Id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                address = user.Address,
                Role = user.Role,
                phone = user.Phone ?? "",
                CreatedAt = DateTime.Now,

            };

            var response = new
            {
                Jwt = jwt,
                userProfile = userProfileDTO

            };

            return Ok(response);

        }


        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound();
            }

            var oldPassword = await _passwordResetRepository.Find(u => u.Email == email);
            if (oldPassword != null)
            {
              await _passwordResetRepository.DeleteAsync(oldPassword);
            }
            // create Password Reset Token
            string token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var passReset = new PasswordReset()
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.Now
            };
            try
            {
                await _passwordResetRepository.AddAsync(passReset);

            }
            catch (Exception )
            {
                return StatusCode(500);
            }

            // send the Password Reset Token by email to the user
            string emailSubject = "Password Reset";
            string username = user.FirstName + " " + user.LastName;
            string emailMessage = "Dear " + username + "\n" +
                "We received your password reset request.\n" +
                "Please copy the following token and paste it in the Password Reset Form:\n" +
                token + "\n\n" +
                "Best Regards\n";

            try
            {
                await _emailSender.SendEmail(emailSubject, email, username, emailMessage);

            }
            catch(Exception)
            {
                return StatusCode(500, "");
            }

            return Ok();
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string password)
        {
            var passReset = await _passwordResetRepository.Find(r => r.Token == token);

            if (passReset == null)
            {
                ModelState.AddModelError("Token", "Wrong Expired Token");
                return BadRequest(ModelState);
            }

            var user = await _userRepository.GetUserByEmail(passReset.Email);
            if (user == null)
            {
                ModelState.AddModelError("Token", "Wrong or Expired Token");
                return BadRequest(ModelState);
            }

            var passwordHasher = new PasswordHasher<User>();
            string encryptedPassword = passwordHasher.HashPassword(new User(), password);

            user.Password = encryptedPassword;

            await _passwordResetRepository.DeleteAsync(passReset);
            return Ok();
        }


        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return Unauthorized();
            }
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower() == "id");
            if (claim == null) return Unauthorized();

            int id;
            try
            {
                id = int.Parse(claim.Value);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
            var user = await _baseRepository.GetByIdAsync(id);
            if (user == null)
            { return Unauthorized(); }

            var userProfile = new UserProfileDTO()
            {
                Id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email,
                address = user.Address,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                phone = user.Phone,
            };
            return Ok(userProfile);
        }


        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDTO userDTO)
        {
            int id = JWTReader.GetUserId(User); // read id of the user from the token
            var user = await _baseRepository.GetByIdAsync(id);

            if (user == null)
            {
                return Unauthorized();
            }

            // update the data 
            try
            {
                user.FirstName = userDTO.firstName;
                user.LastName = userDTO.lastName;
                user.Email = userDTO.email;
                user.Address = userDTO.address;
                user.Phone = userDTO.phone??"";

                await _baseRepository.UpdateAsync(user);
            }
            catch(Exception)
            {
                return StatusCode(500, "an error occurec while updating user profile");
            }
            return Ok();
        }


        [Authorize]
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([Required, MinLength(8), MaxLength(200)]string password)
        {
            int id = JWTReader.GetUserId(User); // read id of the user from the token
            var user = await _baseRepository.GetByIdAsync(id);
            if (user == null)
                return Unauthorized(); // if no users with this id it's unauthorize

            var passwordHasher = new PasswordHasher<User>();
            string encryptedPassword = passwordHasher.HashPassword(new User(), password);

            user.Password = encryptedPassword;
            await _baseRepository.UpdateAsync(user);
            return Ok();
        }

        //private int GetUserId()
        //{
        //    // read user identity & calims & id of the user
        //    var identity = User.Identity as ClaimsIdentity;
        //    if (identity == null)
        //    {
        //        return -1; // if no user 
        //    }
        //    var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower() == "id");
        //    if (claim == null) return -1; // 

        //    int id;
        //    try
        //    {
        //        id = int.Parse(claim.Value);
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }
        //    return id;
        //} // not practical cause there is another controllers may have to read the Token




        //[Authorize]
        //[HttpGet("GetTokenClaims")]
        //public IActionResult GetTokenClaims() // to read the role from user
        //{
        //    var identity = User.Identity as ClaimsIdentity; // cast it as Claims identity
        //    if (identity != null)
        //    {
        //        Dictionary<string, string> claims = new Dictionary<string, string>();
        //        foreach (var claim in identity.Claims)
        //        {
        //            claims.Add(claim.Type, claim.Value);
        //        }
        //        return Ok(claims);
        //    }
        //    return Ok();
        //}




        // CreateToken (JWT) --> Search ?

        #region TestAuthorization
        //[Authorize]
        //[HttpGet("AuthorizeAuthenticatedUsers")] // for all users
        //public IActionResult AuthorizeAuthenticatedUsers()
        //{
        //    return Ok("You are Authorized");
        //}

        //[Authorize(Roles = "admin")] // for admins
        //[HttpGet("AuthorizeAdmin")]
        //public IActionResult AuthorizeAdmin()
        //{
        //    return Ok("You are Authorized");
        //}

        //[Authorize(Roles = "admin, seller")] // for admins and sellers
        //[HttpGet("AuthorizeAdminAndSeller")]
        //public IActionResult AuthorizeAdminAndSeller()
        //{
        //    return Ok("You are Authorized");
        //}
        #endregion

        //[HttpGet("TestToken")]
        //public IActionResult TestToken()
        //{
        //    User user = new User()
        //    {
        //        Id = 500,
        //        Role = "admin"
        //    };
        //    string jwt = CreateJwToken(user);
        //    var response = new
        //    {
        //        Token = jwt,
        //    };
        //    return Ok(response);
        //}


    }
}
