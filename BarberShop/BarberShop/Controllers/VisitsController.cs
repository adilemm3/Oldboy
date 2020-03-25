using BarberShop.Entities;
using BarberShop.Exeptions;
using BarberShop.Exeptions.Throws;
using BarberShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using BarberShop.Services.interfaces;
using System.Security.Claims;


namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    public class VisitsController : Controller
    {
        private readonly ICrudService<Visit> _crudService;
        private readonly ICrudService<Client> _clientCrudService;
        private readonly IMasterServiceCrudService<MasterServices> _masterServiceCrudService;
        private readonly ICrudService<Master> _masterCrudService;
        private readonly IServiceInVisitCrudService<ServiceInVisit> _serviceInVisitCrudService;
        private readonly ICrudService<Service> _serviceCrudService;

        public VisitsController(ICrudService<Visit> crudService,
            ICrudService<Client> clientCrudService, 
            IMasterServiceCrudService<MasterServices> masterServiceCrudService,
            ICrudService<Master> masterCrudService,
            IServiceInVisitCrudService<ServiceInVisit> serviceInVisitCrudService,
            ICrudService<Service> serviceCrudService)
        {
            _crudService = crudService;
            _clientCrudService = clientCrudService;
            _masterServiceCrudService = masterServiceCrudService;
            _masterCrudService = masterCrudService;
            _serviceInVisitCrudService = serviceInVisitCrudService;
            _serviceCrudService = serviceCrudService;
        }
        /// <summary>
        /// Возвращает посещение по id
        /// </summary>
        /// <param name="visitId">id поcещения</param>
        /// <returns></returns>
        [HttpGet("Update")]
        public IActionResult Update(Guid visitId)
        {
            var visit = new Visit();
            try
            {
                ViewBag.sm = new SelectList(_clientCrudService.GetAll(), "Id", "Name");
                visit = _crudService.Get(visitId);
                ViewBag.ClientName = _clientCrudService.GetAll().Select(x=>x.Name).ToList();
                ViewBag.Masters = _masterServiceCrudService.GetAll().Select(x => x.Master.FullName).Distinct().ToList();
                ViewBag.Service = _masterServiceCrudService.GetAll().Where(x => x.Master.FullName == ViewBag.Masters[0]).Select(x => x.Service.NameOfService).ToList();
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(visit);
        }
        /// <summary>
        /// Возвращает все поcещения
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll(string userName)
        {
            var visits = new List<Visit>();
            try
            {
                if (userName!="empty")
                {
                    visits = _crudService.GetAll();
                    if (!String.IsNullOrEmpty(userName))
                    {
                        visits = visits.FindAll(x=>x.Client.Name.Contains(userName)).ToList();
                    }
                }
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(visits);
        }

        [HttpGet("Create")]
        public IActionResult Create(string name)
        {
            var visit = new Visit();
            visit.Client = _clientCrudService.GetAll().FirstOrDefault(x => x.Name == name);
            visit.ClientId = visit.Client.Id;
            //ViewBag.Client = new SelectList(_clientCrudService.GetAll(), "Id", "Name");
            return View(visit);
        }

        /// <summary>
        /// Создать поcещение
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create( Visit visit)
        {
            //DateOfVisitDateValidation dateOfVisitDateValidation = new DateOfVisitDateValidation();
            if (ModelState.IsValid)
            {
                var createdVisit = new List<Visit>();

                try
                {
                    createdVisit.Add(_crudService.Create(visit));
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(createdVisit);
                }
                return RedirectToAction("Get","ServicesInVisits",new { visitId = visit.Id});
            }
            visit.Client = _clientCrudService.Get(visit.ClientId);
            return View(visit);
        }

        /// <summary>
        /// Обновить посещение
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public IActionResult Update(Visit visit)
        {
            if (ModelState.IsValid)
            {
                var updatedVisit = new List<Visit>();

                try
                {                    
                    _crudService.Update(visit);
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    ViewBag.ClientName = _clientCrudService.GetAll().Select(x => x.Name).ToList();
                    return View(visit);
                }
                return RedirectToAction("GetAll", new { userName = _clientCrudService.Get(visit.ClientId).Name });
            }
            return View(visit);
        }

        /// <summary>
        /// Удалить посещение
        /// </summary>
        /// <param name="visitId"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public IActionResult Delete(Guid visitId)
        {
            var userName = _crudService.Get(visitId).Client.Name;
            try
            {
                _crudService.Delete(visitId);
            }
            catch (Exception)
            {
                ViewBag.Error = "Не удалось удалить посещение";
                return RedirectToAction("GetAll");
            }

            return RedirectToAction("GetAll", new { userName = userName });
        }

        [HttpGet("NotAuthorizedUser")]
        public IActionResult NotAuthorizedUser()
        {
            return View();
        }
    }
}