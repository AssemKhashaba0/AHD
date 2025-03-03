using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AHD.Controllers
{
    [Authorize(Roles = "Admin,SubAdmin")]

    public class ProductController : Controller
    {

        private readonly IRepository<Product> _productRepository;

        public ProductController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Invalid file type. Allowed types are .jpg, .jpeg, .png, and .gif.");
                    return View(product);
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/products/" + fileName;
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Product image is required.");
                return View(product);
            }

            _productRepository.Create(product);
            _productRepository.Commit();

            TempData["success"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ImageFile", "Invalid file type. Allowed types are .jpg, .jpeg, .png, and .gif.");
                    return View(product);
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "/images/products/" + fileName;
            }

            _productRepository.Edit(product);
            _productRepository.Commit();

            TempData["success"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();

            _productRepository.Delete(product);
            _productRepository.Commit();

            TempData["success"] = "Product deleted successfully.";
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public IActionResult Inventory()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        [HttpPost]
        public IActionResult UpdateInventory(int id, int quantity)
        {
            var product = _productRepository.GetById(id);
            if (product == null) return NotFound();

            product.Stock = quantity;
            _productRepository.Edit(product);
            _productRepository.Commit();

            TempData["success"] = "Inventory updated successfully.";
            return RedirectToAction(nameof(Inventory));
        }

    }
}

