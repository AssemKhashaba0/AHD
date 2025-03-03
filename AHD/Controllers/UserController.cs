using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Utility.ViewModel;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Agendly.Controllers
{
    public class UserController : Controller
    {
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<IdentityRole> _roleRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserController(
     IRepository<ApplicationUser> userRepository,
     IRepository<IdentityRole> roleRepository,
     IRepository<Order> orderRepository,
     UserManager<ApplicationUser> userManager,
     RoleManager<IdentityRole> roleManager,
     SignInManager<ApplicationUser> signInManager) // إضافة SignInManager كمعلمة
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager; // تهيئة SignInManager
        }

        public IActionResult Index()
        {
            var users = _userRepository.GetAll();
            return View(users);
        }

        public IActionResult Details(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = _userRepository.GetOne(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var orders = _orderRepository.Get(expression: o => o.UserId == userId).ToList();

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                Orders = orders
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlockUser(string userId)
        {
            var user = _userRepository.GetOne(u => u.Id == userId);
            if (user == null)
                return NotFound();

            user.LockoutEnd = DateTime.UtcNow.AddDays(30);
            _userRepository.Edit(user);
            _userRepository.Commit();

            TempData["Message"] = "User has been blocked.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UnblockUser(string userId)
        {
            var user = _userRepository.GetOne(u => u.Id == userId);
            if (user == null)
                return NotFound();

            user.LockoutEnd = null;
            _userRepository.Edit(user);
            _userRepository.Commit();

            TempData["Message"] = "User has been unblocked.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageRoles()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<UserWithRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserWithRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            var rolesList = _roleManager.Roles.Select(r => r.Name).ToList();

            var viewModel = new ManageRolesViewModel
            {
                Users = userRoles,
                Roles = rolesList
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // إزالة الأدوار الحالية
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // إضافة الدور الجديد
            await _userManager.AddToRoleAsync(user, roleName);

            // إذا كان المستخدم الحالي هو الذي تم تغيير صلاحياته، قم بإعادة تسجيل دخوله
            if (user.Id == _userManager.GetUserId(User))
            {
                await _signInManager.SignOutAsync(); // تسجيل الخروج أولاً
                await _signInManager.SignInAsync(user, isPersistent: false); // تسجيل الدخول مرة أخرى
            }

            TempData["Message"] = "Role assigned successfully!";
            return RedirectToAction(nameof(ManageRoles));
        }
    }
}
