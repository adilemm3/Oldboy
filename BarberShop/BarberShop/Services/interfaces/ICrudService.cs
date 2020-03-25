using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarberShop.Services
{
    public interface ICrudService<TResource>
    {
        TResource Get(Guid resourceId);
        List<TResource> GetAll();
        TResource Create(TResource resource);
        TResource Update(TResource resource);
        TResource Delete(Guid resourceid);
    }
}
