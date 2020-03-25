using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Entities
{
    public class MasterServices
    {
        public MasterServices()
        {
            ServicesInVisit = new HashSet<ServiceInVisit>();
        }

        [Key]
        public Guid MasterId { get; set; }
        [Key]
        public Guid ServiceId { get; set; }

        public virtual Master Master { get; set; }

        public virtual Service Service { get; set; }

        public virtual ICollection<ServiceInVisit> ServicesInVisit { get; set; }

    }
}
