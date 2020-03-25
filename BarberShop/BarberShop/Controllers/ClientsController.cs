using BarberShop.Entities;
using BarberShop.Exeptions;
using BarberShop.Exeptions.Throws;
using BarberShop.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class ClientsController : Controller
    {
        private readonly ICrudService<Client> _crudService;

        public ClientsController(ICrudService<Client> crudService)
        {
            _crudService = crudService;
        }

        /// <summary>
        /// Обновление клиента
        /// </summary>
        /// <param name="clientId">id клиента</param>
        /// <returns></returns>
        [HttpGet("Update")]
        public IActionResult Update(Guid clientId)
        {
            var client = new Client();
            try
            {
                client = _crudService.Get(clientId);

            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(client);
        }

        /// <summary>
        /// Возвращает всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll(string name, string phone)
        {
            var clients = new List<Client>();
            try
            {
                clients =_crudService.GetAll();
                if (!String.IsNullOrEmpty(name))
                {
                    clients = clients.FindAll(x => x.Name.Contains(name));
                }
                if (!String.IsNullOrEmpty(phone))
                {
                    clients = clients.FindAll(x => x.Phone.Contains(phone));
                }

            }
            catch (ServiceOperationException exception)
            {
                ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
            }
            return View(clients);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Создание клиента
        /// </summary>
        /// <param name="client">клиент</param>
        /// <returns></returns>
        [HttpPost("Create")]
        public IActionResult Create( Client client)
        {
            if (ModelState.IsValid)
            {
                var createdMaster = new List<Client>();
                var error = new List<Error>();

                try
                {
                    createdMaster.Add(_crudService.Create(client));
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(client);
                }

                return RedirectToAction("GetAll");
            }
            return View(client);
        }
        /// <summary>
        /// Обновить клиента
        /// </summary>
        /// <param name="client">клиент</param>
        /// <returns></returns>
        [HttpPost("Update")]
        public IActionResult Update( Client client)
        {
            if (ModelState.IsValid)
            {
                var updatedClient = new List<Client>();

                try
                {
                    updatedClient.Add(_crudService.Update(client));
                }
                catch (ServiceOperationException exception)
                {
                    ViewBag.Error = ErrorFactory.IdentifyExceptionByType(exception).Description;
                    return View(client);
                }
                return RedirectToAction("GetAll");
            }
            return View(client);
        }

        /// <summary>
        /// Удалить клиента
        /// </summary>
        /// <param name="clientId">id клиента</param>
        /// <returns></returns>
        [HttpDelete("{ClientId:guid}")]
        public IActionResult Delete(Guid clientId)
        {
            var deletedClient = new List<Client>();

            try
            {
                deletedClient.Add(_crudService.Delete(clientId));
            }
            catch (Exception)
            {
                throw new Exception("Не удалось удалить клиента");
            }

            return Json(new { Data = deletedClient });
        }

    }
}
