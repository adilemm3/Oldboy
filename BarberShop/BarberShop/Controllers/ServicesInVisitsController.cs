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

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    public class ServicesInVisitsController : Controller
    {
        private readonly ICrudService<Visit> _crudService;
        private readonly ICrudService<Client> _clientCrudService;
        private readonly IMasterServiceCrudService<MasterServices> _masterServiceCrudService;
        private readonly ICrudService<Master> _masterCrudService;
        private readonly IServiceInVisitCrudService<ServiceInVisit> _serviceInVisitCrudService;
        private readonly ICrudService<Service> _serviceCrudService;

        public ServicesInVisitsController(ICrudService<Visit> crudService,
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
        [HttpGet("Get")]
        public IActionResult Get(Guid visitId)
        {
            var servicesInVisit = new List<ServiceInVisit>();
            try
            {
                servicesInVisit.AddRange(_serviceInVisitCrudService.GetInVisit(visitId));
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            ViewBag.VisitId = visitId;
            var visit = _crudService.Get(visitId);
            ViewBag.TotalCost = visit.TotalCost;
            ViewBag.Date = visit.DateOfVisit;
            return View(servicesInVisit);
        }

        [HttpGet("Create")]
        public IActionResult Create(Guid visitId)
        {
            var serviceInVisit = new ServiceInVisit();
            try
            {
                var masters = _masterCrudService.GetAll();
                ViewBag.Masters = new SelectList(_masterCrudService.GetAll(), "Id", "FullName");
                serviceInVisit.VisitId = visitId;
                ViewBag.Services = new SelectList(_masterServiceCrudService.GetAll().Where(x => x.Master.FullName == masters.FirstOrDefault().FullName).Select(x=>x.Service).ToList(), "Id", "NameOfService");
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(serviceInVisit);
        }

        /// <summary>
        /// Создать поcещение
        /// </summary>
        /// <param name="visit"></param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create(ServiceInVisit serviceInVisit)
        {
            var createdVisit = new ServiceInVisit();

            try
            {
                ViewBag.Masters = new SelectList(_masterCrudService.GetAll(), "Id", "FullName");
                ViewBag.Services = new SelectList(_masterServiceCrudService.GetAll().Where(x => x.Master.Id == serviceInVisit.MasterId).Select(x => x.Service).ToList(), "Id", "NameOfService");
                createdVisit = _serviceInVisitCrudService.Create(serviceInVisit);
                UpdateVisitTotalCost(serviceInVisit.VisitId);
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return View(serviceInVisit);
            }

            return RedirectToAction("Get","ServicesInVisits",new {visitId=  serviceInVisit.VisitId });
        }

        [HttpGet("Edit")]
        public IActionResult Edit(Guid id)
        {
            var serviceInVisit = new ServiceInVisit();
            try
            {
                var masters = _masterCrudService.GetAll();
                ViewBag.Masters = new SelectList(_masterCrudService.GetAll(), "Id", "FullName");
                serviceInVisit = _serviceInVisitCrudService.GetById(id);
                ViewBag.Services = new SelectList(_masterServiceCrudService.GetAll().Where(x => x.Master.Id == serviceInVisit.MasterId).Select(x => x.Service).ToList(), "Id", "NameOfService");
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(serviceInVisit);
        }

        [HttpPost("Edit")]
        public IActionResult Edit(ServiceInVisit serviceInVisit)
        {
            var updatedVisit = new ServiceInVisit();

            try
            {
                ViewBag.Masters = new SelectList(_masterCrudService.GetAll(), "Id", "FullName");
                ViewBag.Services = new SelectList(_masterServiceCrudService.GetAll()
                    .Where(x => x.Master.Id == serviceInVisit.MasterId)
                    .Select(x => x.Service).ToList(), 
                    "Id", 
                    "NameOfService");
                _serviceInVisitCrudService.Update(serviceInVisit);
                UpdateVisitTotalCost(serviceInVisit.VisitId);
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return View(serviceInVisit);
            }

            return RedirectToAction("Get", "ServicesInVisits", new { visitId = serviceInVisit.VisitId });
        }

        [HttpGet("Delete")]
        public IActionResult Delete(Guid id)
        {
            var serviceInVisit = new ServiceInVisit{Id = id};
            try
            {
                var visitId = _serviceInVisitCrudService.GetById(id).VisitId;
                serviceInVisit = _serviceInVisitCrudService.Delete(id);
                UpdateVisitTotalCost(visitId);
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return View(serviceInVisit);
            }
            return RedirectToAction("Get", "ServicesInVisits", new { visitId = serviceInVisit.VisitId });
        }

        private void UpdateVisitTotalCost(Guid ServiceInVisitId)
        {
            var visit = _crudService.Get(ServiceInVisitId);
            var servicesInVisit = _serviceInVisitCrudService.GetInVisit(ServiceInVisitId);
            visit.TotalCost = servicesInVisit.Sum(x => x.MasterServices.Service.Price);
            ViewBag.TotalCost = visit.TotalCost;
            _crudService.Update(visit);
        }

        [HttpGet("GetServices")]
        public ActionResult GetServices(string id)
        {
            ViewBag.MasterServices = new SelectList(_masterServiceCrudService.GetAll().Where(x => x.Master.Id == Guid.Parse(id)).Select(x => x.Service).ToList(), "Id", "NameOfService");
            return PartialView();
        }
    }
}