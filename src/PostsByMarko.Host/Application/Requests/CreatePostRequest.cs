using System.ComponentModel.DataAnnotations;

﻿namespace PostsByMarko.Host.Application.Requests
{
    public class CreatePostRequest
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [Required, StringLength(20000)]
        public string Content {  get; set; } = string.Empty;
    }
}
