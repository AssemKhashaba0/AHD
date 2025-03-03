using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Utility;

namespace AHD.Controllers
{

    [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

    public class MosqueController : Controller
    {
        private readonly MosqueIRepository _mosqueRepository;
        private readonly CityIRepository _cityRepository;

        public MosqueController(MosqueIRepository mosqueRepository, CityIRepository cityRepository)
        {
            _mosqueRepository = mosqueRepository;
            _cityRepository = cityRepository;
        }

        public IActionResult Index()
        {
            var mosques = _mosqueRepository.GetAll("City");
            return View(mosques);
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        public IActionResult Create()
        {
            var cities = _cityRepository.GetAll(); // افترض أن هذا يعيد List<City>
            ViewBag.Cities = cities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(), // القيمة التي سيتم إرسالها عند التحديد
                Text = c.Name // النص المعروض في القائمة المنسدلة
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        public IActionResult Create(Mosque mosque)
        {
            if (!ModelState.IsValid)
            {
                // طباعة أخطاء التحقق
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // إعادة تعيين ViewBag.Cities
                var cities = _cityRepository.GetAll();
                ViewBag.Cities = cities.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

                return View(mosque);
            }

            _mosqueRepository.Create(mosque);
            _mosqueRepository.Commit();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        public IActionResult Edit(int id)
        {
            var mosque = _mosqueRepository.GetById(id);
            if (mosque == null)
                return NotFound();

            // تحويل قائمة المدن إلى SelectListItem
            var cities = _cityRepository.GetAll();
            ViewBag.Cities = cities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(mosque);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        public IActionResult Edit(Mosque mosque)
        {
            if (ModelState.IsValid)
            {
                _mosqueRepository.Edit(mosque);
                _mosqueRepository.Commit();
                return RedirectToAction(nameof(Index));
            }

            // تحويل قائمة المدن إلى SelectListItem
            var cities = _cityRepository.GetAll();
            ViewBag.Cities = cities.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(mosque);
        }
        [Authorize(Roles = $"{SD.AdminRoles},{SD.SubAdminRoles}")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                var mosque = _mosqueRepository.GetById(id);
                if (mosque == null)
                {
                    return Json(new { success = false, message = "المسجد غير موجود." });
                }

                _mosqueRepository.Delete(mosque);
                _mosqueRepository.Commit();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء الحذف: " + ex.Message });
            }
        }
    }
}