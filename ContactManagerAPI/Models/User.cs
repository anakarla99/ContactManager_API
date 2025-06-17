using System;
using System.ComponentModel.DataAnnotations;

namespace ContactManagerApi.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; } = null!;

        [Required]
        [MaxLength(60)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string Password { get; set; } = null!;
    }
}
