using BarberShop.DataStorages;
using BarberShop.Entities;
using BarberShop.Exeptions;
using BarberShop.Exeptions.Throws;
using BarberShop.Models;
using BarberShop.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    public class MastersController : Controller
    {
        private readonly ICrudService<Master> _crudService;
        private Guid userId = new Guid("60a69d44-f6fd-4915-b039-d064b9e3934f");
        private readonly BarberShopContext _context;
        public MastersController(ICrudService<Master> crudService, BarberShopContext context)
        {
            _crudService = crudService;
            _context = context;
        }


        /// <summary>
        /// Возвращает мастера по id
        /// </summary>
        /// <param name="masterId">id мастера</param>
        /// <returns></returns>
        //[HttpGet("{masterId:guid}")]
        public IActionResult Get(Guid masterId)
        {
            var master = new Master();
            try
            {
                master=_crudService.Get(masterId);

            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(master);
        }

        /// <summary>
        /// Возвращает всех мастеров
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var masters = new List<Master>();
            try
            {
                masters.AddRange(_crudService.GetAll());

            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(masters.ToArray());
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Создать мастера
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create( MasterViewModel master)
        {
            if (ModelState.IsValid)
            {
                var createdMaster = new List<Master>();
                var error = new List<Error>();

                try
                {
                    createdMaster.Add(_crudService.Create(master));
                    User user = new User { Phone = master.Phone, Password = master.Password, FullName = master.FullName, RoleId= userId };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(master);
                }

                return RedirectToAction("Get", "MasterServices", new { masterId = master.Id  });
            }
            return View(master);
        }
        /// <summary>
        /// Обновить мастера
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public IActionResult Update( Master master)
        {
            if (ModelState.IsValid)
            {
                var updatedMaster = new List<Master>();

                try
                {
                    updatedMaster.Add(_crudService.Update(master));
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(master);
                }
                return RedirectToAction("GetAll");
            }
            return View(master);
        }

        /// <summary>
        /// Удалить мастера
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        //[HttpDelete("{masterId:guid}")]
        [HttpGet("Delete")]
        public IActionResult Delete(Guid masterId)
        {
            var deletedMaster = new List<Master>();

            try
            {
                deletedMaster.Add(_crudService.Delete(masterId));
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }

            return RedirectToAction("GetAll");
        }
        
    }
}