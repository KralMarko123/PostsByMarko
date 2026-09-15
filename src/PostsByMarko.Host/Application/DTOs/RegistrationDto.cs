using System.ComponentModel.DataAnnotations;

﻿namespace PostsByMarko.Host.Application.DTOs
{
    public class RegistrationDto
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(256, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
