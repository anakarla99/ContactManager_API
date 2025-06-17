using ContactManagerApi.Models;
using ContactManagerApi.DTOs;
using ContactManagerApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManagerApi.Data;
using Microsoft.AspNetCore.Authorization;

namespace ContactManagerApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactContext _context;
        private readonly IContactService _service;

        public ContactsController(ContactContext context, IContactService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var contacts = await _service.GetContactsAsync();
            return Ok(contacts);
        }

        // GET: api/contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(Guid id)
        {
            var contact = await _service.GetContactAsync(id);
            if (contact == null)
                return NotFound();
            return Ok(contact);
        }
        // POST: api/contacts
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
        
            var username = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (username == null)
                return BadRequest(new { error = "User claim not found." });
        
            var (success, error, createdContact) = await _service.CreateContactAsync(contact, username);
            if (!success)
                return BadRequest(new { error });
        
            return CreatedAtAction(nameof(GetContact), new { id = createdContact!.Id }, createdContact);
        }

        // PUT: api/contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(Guid id, Contact contact)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, error) = await _service.UpdateContactAsync(id, contact);
            if (!success)
                return BadRequest(new { error });

            return NoContent();
        }

        // DELETE: api/contacts/5
        [Authorize(Policy = "CubanAdminsOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(Guid id)
        {
            var (success, error) = await _service.DeleteContactAsync(id);
            if (!success)
                return NotFound(new { error });
        
            return NoContent();
        }
    }
}
