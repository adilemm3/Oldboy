using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BarberShop.Entities
{
    public class User: BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(13)]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
