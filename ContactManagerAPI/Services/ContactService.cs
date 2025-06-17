using ContactManagerApi.Models;
using ContactManagerApi.DTOs;
using ContactManagerApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApi.Services
{
    public class ContactService : IContactService
    {
        private readonly ContactContext _context;

        public ContactService(ContactContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContactDto>> GetContactsAsync()
        {
            var contacts = await _context.Contacts.ToListAsync();
            return contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                DateOfBirth = c.DateOfBirth,
                Age = c.Age,
                Phone = c.Phone,
                Owner = c.Owner
            });
        }

        public async Task<ContactDto?> GetContactAsync(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null) return null;
            return new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                DateOfBirth = contact.DateOfBirth,
                Age = contact.Age,
                Phone = contact.Phone,
                Owner = contact.Owner
            };
        }

        public async Task<(bool Success, string? Error, Contact? Contact)> CreateContactAsync(Contact contact, string username)
        {
            // Validación de modelo (campos requeridos, longitud, email) se hace en el controlador con ModelState

            // Validar unicidad de email
            if (await _context.Contacts.AnyAsync(c => c.Email == contact.Email && c.Owner == contact.Owner))
                return (false, "Email must be unique.", null);

            // Validar mayoría de edad (18+)
            var today = DateTime.Today;
            var age = today.Year - contact.DateOfBirth.Year;
            if (contact.DateOfBirth.Date > today.AddYears(-age)) age--;
            if (age < 18)
                return (false, "Contact must be at least 18 years old.", null);

            // Obtener el usuario autenticado
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return (false, "User not found.", null);

            contact.Owner = user.Id;

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return (true, null, contact);
        }

        public async Task<(bool Success, string? Error)> UpdateContactAsync(Guid id, Contact contact)
        {
            if (id != contact.Id)
                return (false, "Id mismatch.");

            var existing = await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (existing == null)
                return (false, "Contact not found.");


            // Validar unicidad de email (excepto el propio contacto)
            if (await _context.Contacts.AnyAsync(c => c.Email == contact.Email && c.Owner == contact.Owner))
                return (false, "Email must be unique.");

            // Validar mayoría de edad (18+)
            var today = DateTime.Today;
            var age = today.Year - contact.DateOfBirth.Year;
            if (contact.DateOfBirth.Date > today.AddYears(-age)) age--;
            if (age < 18)
                return (false, "Contact must be at least 18 years old.");
            
            // No permitir cambiar el Owner
            contact.Owner = existing.Owner;

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Contacts.AnyAsync(e => e.Id == id))
                    return (false, "Contact not found.");
                else
                    throw;
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteContactAsync(Guid id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
                return (false, "Contact not found.");

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}