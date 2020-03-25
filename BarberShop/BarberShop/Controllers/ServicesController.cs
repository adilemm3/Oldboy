using BarberShop.Entities;
using BarberShop.Exeptions;
using BarberShop.Exeptions.Throws;
using BarberShop.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    public class ServicesController : Controller
    {
        private readonly ICrudService<Service> _crudService;

        public ServicesController(ICrudService<Service> crudService)
        {
            _crudService = crudService;
        }

        /// <summary>
        /// Возвращает мастера по id
        /// </summary>
        /// <param name="serviceId">id мастера</param>
        /// <returns></returns>
        [HttpGet("Edit")]
        public IActionResult Edit(Guid serviceId)
        {
            var service = new Service();
            try
            {
                service = _crudService.Get(serviceId);

            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(service);
        }

        /// <summary>
        /// Возвращает всех услуги
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var services = new List<Service>();
            try
            {
                services.AddRange(_crudService.GetAll());

            }
            catch (Exception)
            {
                throw new Exception("Не удалось получить услугу");
            }
            return View(services);
        }

        /// <summary>
        /// Создать услугу
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create(Service service)
        {
            var createdService = new List<Service>();

            try
            {
                createdService.Add(_crudService.Create(service));
            }
            catch (Exception)
            {
                throw new Exception("Не удалось добавить услугу");
            }

            return Json(new { Data = createdService });
        }

        /// <summary>
        /// Обновить мастера
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public IActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                var updatedService = new Service();

                try
                {
                    updatedService=_crudService.Update(service);
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(service);
                }
                return RedirectToAction("GetAll");
            }
            return View(service);
        }

        /// <summary>
        /// Удалить услугу
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpDelete("{ServiceId:guid}")]
        public IActionResult Delete(Guid serviceId)
        {
            var deletedService = new List<Service>();

            try
            {
                deletedService.Add(_crudService.Delete(serviceId));
            }
            catch (Exception)
            {
                throw new Exception("Не удалось удалить услугу");
            }

            return Json(new { Data = deletedService });
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
    }
}