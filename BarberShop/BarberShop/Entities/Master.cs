using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.DataAnnotations;


namespace BarberShop.Entities
{
    [Table(name:"Barbers")]
    public class Master : BaseEntity
    {
        [Required(ErrorMessage = "Введите ФИО")]
        [StringLength(100)]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "Введите номер телефона")]
        [MaxLength(13, ErrorMessage = "Введите правильный номер телефона")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введите пол")]
        [StringLength(7)]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Введите процент с услуги")]
        [Range(20, 60,ErrorMessage = "Процент с услуги должен быть в диапазоне от 20 до 60")]
        public double PercentForTheService { get; set; }
        [StringLength(60)]
        public string Email { get; set; }

        public virtual ICollection<MasterServices> MasterServices { get; set; }
        
    }
}
