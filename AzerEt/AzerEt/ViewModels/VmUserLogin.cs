﻿using System.ComponentModel.DataAnnotations;

namespace AzerEt.ViewModels
{
    public class VmUserLogin
    {
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = true;
        public bool IsOnline { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
