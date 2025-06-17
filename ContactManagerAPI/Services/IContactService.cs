using ContactManagerApi.Models;
using ContactManagerApi.DTOs;

namespace ContactManagerApi.Services
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetContactsAsync();
        Task<ContactDto?> GetContactAsync(Guid id);
        Task<(bool Success, string? Error, Contact? Contact)> CreateContactAsync(Contact contact, string username);
        Task<(bool Success, string? Error)> UpdateContactAsync(Guid id, Contact contact);
        Task<(bool Success, string? Error)> DeleteContactAsync(Guid id);
    }
}