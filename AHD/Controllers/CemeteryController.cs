using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace AHD.Controllers
{
    [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

    public class CemeteryController : Controller
    {
        private readonly CemeteryIRepository _cemeteryRepository;
        private readonly CityIRepository _cityRepository;

        public CemeteryController(CemeteryIRepository cemeteryRepository, CityIRepository cityRepository)
        {
            _cemeteryRepository = cemeteryRepository;
            _cityRepository = cityRepository;
        }

        public IActionResult Index()
        {
            var cemeteries = _cemeteryRepository.GetAll("City");
            return View(cemeteries);
        }

        public IActionResult Create()
        {
            ViewBag.Cities = _cityRepository.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cemetery cemetery)
        {
            if (ModelState.IsValid)
            {
                _cemeteryRepository.Create(cemetery);
                _cemeteryRepository.Commit();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cities = _cityRepository.GetAll();
            return View(cemetery);
        }

        public IActionResult Edit(int id)
        {
            var cemetery = _cemeteryRepository.GetById(id);
            if (cemetery == null)
                return NotFound();
            ViewBag.Cities = _cityRepository.GetAll();
            return View(cemetery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cemetery cemetery)
        {
            if (ModelState.IsValid)
            {
                _cemeteryRepository.Edit(cemetery);
                _cemeteryRepository.Commit();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cities = _cityRepository.GetAll();
            return View(cemetery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var cemetery = _cemeteryRepository.GetById(id);
                if (cemetery == null)
                {
                    return Json(new { success = false, message = "المقبرة غير موجودة." });
                }

                _cemeteryRepository.Delete(cemetery);
                _cemeteryRepository.Commit();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء الحذف: " + ex.Message });
            }
        }


    }
}



