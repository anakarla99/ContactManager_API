using Xunit;
using ContactManagerApi.Controllers;
using ContactManagerApi.Models;
using ContactManagerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ContactManagerAPI.Tests
{
    public class ContactsControllerTests
    {
        private ContactContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ContactContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new ContactContext(options);

            // Seed a user
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Password = "testpass"
            });
            context.SaveChanges();

            return context;
        }

        private ContactsController GetController(ContactContext context, string username = "testuser")
        {
            var controller = new ContactsController(context);

            // Mock user identity
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("sub", username)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task PostContact_ReturnsCreated_WhenValid()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var contact = new Contact
            {
                FirstName = "John",
                Email = "john@example.com",
                DateOfBirth = DateTime.Today.AddYears(-20),
                Phone = "123456789"
            };

            var result = await controller.PostContact(contact);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdContact = Assert.IsType<Contact>(createdResult.Value);
            Assert.Equal("John", createdContact.FirstName);
        }

        [Fact]
        public async Task PostContact_ReturnsBadRequest_WhenUnderage()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            var contact = new Contact
            {
                FirstName = "Young",
                Email = "young@example.com",
                DateOfBirth = DateTime.Today.AddYears(-10),
                Phone = "123456789"
            };

            var result = await controller.PostContact(contact);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("18 years old", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetContacts_ReturnsList()
        {
            var context = GetDbContext();
            var controller = GetController(context);

            context.Contacts.Add(new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                Email = "jane@example.com",
                DateOfBirth = DateTime.Today.AddYears(-30),
                Phone = "987654321",
                Owner = context.Users.First().Id
            });
            context.SaveChanges();

            var result = await controller.GetContacts();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(contacts);
        }
    }
}