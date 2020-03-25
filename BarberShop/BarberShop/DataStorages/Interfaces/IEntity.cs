using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarberShop.DataStorages.Interfaces
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}
