using AutoMapper;
using E_CommerceApplication.BLL.DTO;
using E_CommerceApplication.BLL.Interfaces;
using E_CommerceApplication.BLL.Models;
using E_CommerceApplication.DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IBaseRepository<Contact> _baseRepository;
        private readonly IBaseRepository<Subject> _subjectRepository;
        private readonly IContactRepository _contactRepository;
        private readonly EmailSender emailSender;
        

        public ContactsController(IBaseRepository<Contact> baseRepository,
            IBaseRepository<Subject> subjectRepository,
            IContactRepository contactRepository,
            EmailSender emailSender)
        {
            _baseRepository = baseRepository;
            _subjectRepository = subjectRepository;
            _contactRepository = contactRepository;
            this.emailSender = emailSender;
            //_mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        [HttpGet(nameof(GetSubjects))]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _subjectRepository.GetAll();
            if (subjects == null)
                return NotFound("no subjects");
            return Ok(subjects);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int? page)
        {
            if (page == null || page < 1)
            {
                page = 1;
            }

            int pageSize = 5;
            int totalPages = 0;
            decimal count = await _baseRepository.Count();
            totalPages = (int)Math.Ceiling(count / pageSize);

            var contacts = await _contactRepository.GetAllWithSubjects((int)page, pageSize);
            if (contacts == null || !contacts.Any())
            {
                return NotFound("not found");
            }

            var contactsDTO = new List<ContactDTO>()
            {};

            foreach (var contact in contacts)
            {
                contactsDTO.Add(new ContactDTO
                {
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Email = contact.Email,
                    Message = contact.Message,
                    Phone = contact.Phone,
                    subject = contact.Subject,
                });
            }

            var response = new
            {
                Contacts = contactsDTO,
                TotalPages = totalPages,
                pageSize = pageSize,
                page = page ?? 1,
            };
            return Ok(response);
        }


        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID");
            }

            var contact = await _contactRepository.GetContactByIdWithSubjects(id);
            if (contact == null)
            { return NotFound("not found"); }

            var contactDTO = new ContactDTO
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Message = contact.Message,
                Phone = contact.Phone,
                subject = contact.Subject,
            };
            return Ok(contactDTO);
        }


        // any user can create contact 
        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactDTO contactDTO)
        {
            var subject = await _subjectRepository.GetByIdAsync(contactDTO.subject.Id);
            if (subject == null)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }
            if (contactDTO.subject == null || contactDTO.subject.Id == 0)
            {
                ModelState.AddModelError("Subject", "Please select a valid subject");
                return BadRequest(ModelState);
            }
            
            var newContact = new Contact
            {
                FirstName = contactDTO.FirstName,
                LastName = contactDTO.LastName,
                Email = contactDTO.Email,
                Phone = contactDTO.Phone ?? string.Empty,
                Message = contactDTO.Message,
                CreatedAt = DateTime.UtcNow,
                Subject = subject,
            };
            try
            {
                await _baseRepository.AddAsync(newContact);
            }
            catch(Exception)
            {
                return StatusCode(500, "an error occured while adding new contact");
            }
            // send confarmation emil
            string emailSubject = "Contact Confarmation";
            string userName = contactDTO.FirstName + " " + contactDTO.LastName;
            string emailMessage = "Dear " + userName + "\n" +
                "We received you message. Thank you for contacting us.\n" +
                "Our team will contact you very soon.\n" +
                "Best Regards\n\n" +
                "Your Message:\n" + contactDTO.Message;

            // call sendEmail object to send the mail
            await emailSender.SendEmail(emailSubject, contactDTO.Email, userName, emailMessage);

            return Ok(contactDTO);
        }

        
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactDTO contactDTO)
        //{
        //    var contact = await _baseRepository.GetByIdAsync(id);
        //    if (contact == null)
        //    { return NotFound("not found"); }

        //    try
        //    {
        //        contact.Message = contactDTO.Message;
        //        contact.Email = contactDTO.Email;
        //        contact.Phone = contactDTO.Phone ?? "";
        //        contact.FirstName = contactDTO.FirstName;
        //        contact.LastName = contactDTO.LastName;
        //        contact.Subject = contactDTO.subject;

        //        await _baseRepository.UpdateAsync(contact);
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(500,"An error code while updating the contact");
        //    }
        //   return Ok(contactDTO);
        //}


        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = new Contact { Id = id, Subject = new Subject() };
            
            try
            {
                await _baseRepository.DeleteAsync(contact);
            }
            catch(Exception)
            {
                return NotFound("not found");
            }
            return Ok();
        }
    
    }
}
