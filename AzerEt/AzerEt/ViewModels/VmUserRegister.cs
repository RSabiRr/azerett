﻿using System.ComponentModel.DataAnnotations;

namespace AzerEt.ViewModels
{
    public class VmUserRegister
    {
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Surname { get; set; }
    
        [MaxLength(100)]

        public string Phone { get; set; }
        [MaxLength(100)]

        public string Gender { get; set; }
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [MaxLength(100)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RepaetPassword { get; set; }
        public bool IsAdmin { get; set; } = false;
		public bool IsOnline { get; set; } = false;

	}
}
