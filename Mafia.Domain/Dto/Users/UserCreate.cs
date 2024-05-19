﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mafia.Domain.Dto
{
    public class UserCreate
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [Display(Name = "ЛПУ")]
        [Required(ErrorMessage = "ЛПУ обязательно!")]
        public int OrganizationId { get; set; }
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Display(Name = "Роль")]
        public string IdentityRoleId { get; set; }
        [Display(Name = "ФИО")]
        public string FIO { get; set; }
        [Display(Name = "Телефон")]
        public string Phone { get; set; }
        [Display(Name = "ПИН")]
        public string Pin { get; set; }

    }
}
