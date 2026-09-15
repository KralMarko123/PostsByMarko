using System.ComponentModel.DataAnnotations;

﻿namespace PostsByMarko.Host.Application.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string Password { get; set; } = string.Empty;
    }
}
