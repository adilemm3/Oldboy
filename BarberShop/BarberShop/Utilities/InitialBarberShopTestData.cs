using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.DataStorages;
using BarberShop.DataStorages.Interfaces;

namespace BarberShop.Utilities
{
    public static class InitialBarberShopTestData
    {
        public static void Initialize(IBarberShopStorage storage)
        {
            if (storage.Masters.Get().Any())
                return;

            storage.Masters.Add(
                    new Entities.Master
                    {
                        FullName = "Кирилов Виктор Сергеевич",
                        Phone = "89234163110",
                        PercentForTheService = 50,
                        Email = ""
                    }
                );
            storage.Save();

        }
    }
}
