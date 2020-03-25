using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Entities
{
    public class ServiceInVisit:BaseEntity
    {
        [Required]
        public Guid MasterId { get; set; }
        [Required]
        public Guid ServiceId { get; set; }
        [Required]
        public Guid VisitId { get; set; }

        public virtual MasterServices MasterServices { get; set; }

        public virtual Visit Visit { get; set; }
    }
}
