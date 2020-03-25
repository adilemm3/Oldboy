using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BarberShop.DataStorages.Interfaces;
using BarberShop.Services;
using BarberShop.Entities;
using BarberShop.Services.interfaces;
using BarberShop.DataStorages;
using BarberShop.Exeptions.Throws;
using BarberShop.Exeptions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    public class MasterServicesController:Controller
    {
        private readonly IMasterServiceCrudService<MasterServices> _masterServiceCrudService;
        private readonly ICrudService<Service> _crudService;
        private readonly ICrudService<Master> _masterCrudService;
        private readonly ICrudService<Visit> _visitCrudService;
        private readonly IServiceInVisitCrudService<ServiceInVisit> _serviceInVisitCrudService;

        public MasterServicesController(IMasterServiceCrudService<MasterServices> masterServiceCrudService,
            ICrudService<Service> crudService,
            ICrudService<Master> masterCrudService,
            ICrudService<Visit> visitCrudService,
            IServiceInVisitCrudService<ServiceInVisit> serviceInVisitCrudService)
        {
            _masterServiceCrudService = masterServiceCrudService;
            _crudService = crudService;
            _masterCrudService = masterCrudService;
            _visitCrudService = visitCrudService;
            _serviceInVisitCrudService = serviceInVisitCrudService;
        }

        /// <summary>
        /// Возвращает услугу мастера по id
        /// </summary>
        /// <param name="masterId">id мастера</param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        //[HttpGet("{masterId:guid}/{serviceId:guid}")]
        [HttpGet("Edit")]
        public IActionResult Edit(Guid masterId,Guid serviceId)
        {
            var master = new MasterServices();
            try
            {
                ViewBag.Servises = _crudService.GetAll().ToList();
                ViewBag.EditServiseName = _crudService.GetAll().Select(x => x.NameOfService).ToList();
                master = _masterServiceCrudService.Get(masterId, serviceId);
                ViewBag.MasterServices = master;
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(master);
        }
        /// <summary>
        /// Изменить услугу парикмахера
        /// </summary>
        /// <param name="masterServices"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        public IActionResult Edit(MasterServices masterServices)
        {
            try
            {
                ViewBag.EditServiseName = _crudService.GetAll().Select(x => x.NameOfService).ToList();
                //masterServices.Master = _masterCrudService.Get(masterServices.MasterId);
                var service = _crudService.GetAll().First(x => x.NameOfService == masterServices.Service.NameOfService);
                masterServices.Service = service;
                if (_masterServiceCrudService.Get(masterServices.MasterId, masterServices.Service.Id) == null)
                {
                    var deletedService = _masterServiceCrudService.Delete(masterServices.MasterId, masterServices.ServiceId);
                    masterServices.ServiceId = service.Id;
                    var updatedMasterService = _masterServiceCrudService.UpdateNew(masterServices);
                }
                else
                {
                    if (masterServices.Service.Id != masterServices.ServiceId) masterServices.ServiceId = masterServices.Service.Id;
                    _masterServiceCrudService.Update(masterServices);
                }
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return View(masterServices);
            }
            return RedirectToAction("Get", new { masterId = masterServices.MasterId});
        }
        /// <summary>
        /// вернуть парикмахера по id
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        [HttpGet("Get")]
        public IActionResult Get(Guid masterId)
        {
            var masterServices = new List<MasterServices>();

            try
            {
                ViewBag.FullName = _masterCrudService.Get(masterId).FullName;
                masterServices.AddRange(_masterServiceCrudService.GetByMasterId(masterId));
                ViewBag.MasterId = masterId;
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(masterServices);
        }
        /// <summary>
        /// Добавить услугу парикмахера
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        [HttpGet("Create")]
        public IActionResult Create(Guid masterId)
        {
            var masterIdToCreateMasterService = new MasterServices() { MasterId = masterId };
            ViewBag.Serviсes = new SelectList(_crudService.GetAll(),"Id","NameOfService");
            return View(masterIdToCreateMasterService);
        }
        /// <summary>
        /// Добавить услугу парикмахера
        /// </summary>
        /// <param name="masterServices">услуга мастера</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create( MasterServices masterServices)
        {
            var createdMaster = new List<MasterServices>();

            try
            {
                ViewBag.Serviсes = new SelectList(_crudService.GetAll(), "Id", "NameOfService");
                createdMaster.Add(_masterServiceCrudService.Create(masterServices));
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return View(masterServices);
            }

            return RedirectToAction("Get", new { masterId = masterServices.MasterId });
        }

        /// <summary>
        /// Удалить услугу парикмахера
        /// </summary>
        /// <param name="masterId"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpGet("Delete")]
        public IActionResult Delete(Guid masterId, Guid serviceId)
        {
            var deletedMaster = new MasterServices { MasterId = masterId, ServiceId = serviceId };
            try
            {
                deletedMaster= _masterServiceCrudService.Delete(masterId,serviceId);
                UpdateAllTotalCost();
            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                return RedirectToAction("Get", new { masterId = deletedMaster.MasterId });
            }

            return RedirectToAction("Get", new { masterId = deletedMaster.MasterId });
        }

        private void UpdateAllTotalCost()
        {
            var visits = _visitCrudService.GetAll();
            foreach (var visit in visits)
            {
                visit.TotalCost = _serviceInVisitCrudService.GetInVisit(visit.Id).Sum(y => y.MasterServices.Service.Price);
                _visitCrudService.Update(visit);
            }
        }
    }
}
