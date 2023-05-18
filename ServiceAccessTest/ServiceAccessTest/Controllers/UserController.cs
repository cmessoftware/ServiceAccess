using Microsoft.AspNetCore.Mvc;
using ServiceAccess.Entities;
using ServiceAccessTest.Entiities;
using System;
using System.Configuration;
using static ServiceAccess.Communication;

namespace WebApiPrueba.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger,
                              IConfiguration configuration)
        {
            this._logger = logger;
            this._configuration = configuration;
        }

        // GET: HomeController

        [HttpGet]
        [Route("/")]
        public async Task<ActionResult<string>> Get()
        {
            var uri = _configuration.GetValue<string>("URI_GET_USERS");
            var comm = new ServiceAccess.Communication(_configuration);
            var response = await comm.GetServiceResponse<User>(HttpUrlMethods.Get, new Uri(uri));

            return Json(response);
        }

        [HttpGet]
        [Route("/{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var uri = _configuration.GetValue<string>("URI_GET_USERS");
            uri = $"{uri}/{id}";

            var comm = new ServiceAccess.Communication(_configuration);
            var response = await comm.GetServiceResponse<User>(HttpUrlMethods.Get, new Uri(uri));

            return Json(response);
         
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
