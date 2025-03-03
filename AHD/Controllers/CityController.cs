using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace AHD.Controllers
{
    [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

    public class CityController : Controller
    {
      
            private readonly IRepository<City> _cityRepository;

            public CityController(IRepository<City> cityRepository)
            {
                _cityRepository = cityRepository;
            }

            public IActionResult Index()
            {
                var cities = _cityRepository.GetAll();
                return View(cities);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            public IActionResult Create(City city)
            {
                if (ModelState.IsValid)
                {
                    _cityRepository.Create(city);
                    _cityRepository.Commit();
                    return RedirectToAction(nameof(Index)); 
                }
                return View(city); 
            }

            public IActionResult Edit(int id)
            {
                var city = _cityRepository.GetById(id);
                if (city == null)
                    return NotFound();

                return View(city);
            }

            [HttpPost]
            public IActionResult Edit(City city)
            {
                if (ModelState.IsValid)
                {
                    _cityRepository.Edit(city);
                    _cityRepository.Commit();
                    return RedirectToAction(nameof(Index)); 
                }
                return View(city);
            }

            public IActionResult Delete(int id)
            {
                var city = _cityRepository.GetById(id);
                if (city == null)
                    return NotFound();

                return View(city);
            }

            [HttpPost]
            public IActionResult DeleteConfirmed(int id)
            {
                var city = _cityRepository.GetById(id);
                if (city == null)
                    return NotFound();

                _cityRepository.Delete(city);
                _cityRepository.Commit();
                return RedirectToAction(nameof(Index)); 
            }
        }

    }
