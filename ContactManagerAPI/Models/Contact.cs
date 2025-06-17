using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManagerApi.Models
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; } = null!;

        [MaxLength(128)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string Email { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        public Guid Owner { get; set; }

        [NotMapped]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }
}
