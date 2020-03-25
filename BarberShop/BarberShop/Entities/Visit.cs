using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BarberShop.Entities
{
    public class Visit : BaseEntity
    {
        public Visit()
        {
            ServicesInVisit = new HashSet<ServiceInVisit>();
        }
        [Required]
        public Guid ClientId { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Итоговая сумма должна быть в диапазоне от 0 до 100000")]
        public int TotalCost { get; set; }

        [Required]
        [Column(TypeName = "smalldatetime")]
        [DateOfVisitDateValidation()]
        public DateTime DateOfVisit { get; set; }

        public virtual ICollection<ServiceInVisit> ServicesInVisit { get; set; }

        [JsonIgnore]
        public virtual Client Client { get; set; }

    }

    public class DateOfVisitDateValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = (DateTime)value;
            if (dateTime<DateTime.Now)
            { 
                ErrorMessage = "Неверная дата посещения";
                return false;
            }
            else if(dateTime.Hour < 10 || dateTime.Hour > 20)
            {
                ErrorMessage = "Укажите время в интервале от 10:00 до 20:00";
                return false;
            }
            return true;
        }
    }
}
