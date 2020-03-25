using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Entities
{
    public class Service : BaseEntity
    {
        [Required]
        [StringLength(30)]
        public string NameOfService { get; set; }

        [Required]
        [Display(Name ="Цена")]
        public int Price { get; set; }
        public virtual ICollection<MasterServices> MasterServices { get; set; }
    }
}
