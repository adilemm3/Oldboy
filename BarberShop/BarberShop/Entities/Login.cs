using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BarberShop.Entities
{
    public class Login
    {
        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(60, ErrorMessage = "ФИО слишком длинное")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
