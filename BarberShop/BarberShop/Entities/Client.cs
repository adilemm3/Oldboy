using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BarberShop.Entities
{
    public class Client : BaseEntity
    {
        [Required(ErrorMessage = "Введите ФИО")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите номер телефона")]
        [MaxLength(13, ErrorMessage = "Введите правильный номер телефона")]
        public string Phone { get; set; }

        public virtual ICollection<Visit> Visits { get; set; }
    }
}
