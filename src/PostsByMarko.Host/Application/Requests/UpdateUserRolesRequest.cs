using System.ComponentModel.DataAnnotations;

﻿using PostsByMarko.Host.Application.Enums;

namespace PostsByMarko.Host.Application.Requests
{
    public class UpdateUserRolesRequest
    {
        [Required]
        public Guid? UserId { get; set; }
        [Required, EnumDataType(typeof(ActionType))]
        public ActionType? ActionType { get; set; }
        [Required, StringLength(256)]
        public string Role { get; set; } = string.Empty;
    }
}
