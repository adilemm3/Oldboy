using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BarberShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BarberShop.Services;
using BarberShop.Entities;
using BarberShop.DataStorages;

namespace BarberShop.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly BarberShopContext _context;
        public AccountController(BarberShopContext context)
        {
            _context = context;
        }
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == model.Phone && u.FullName == model.FullName);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Phone = model.Phone, Password = model.Password, FullName = model.FullName };
                    Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.Users.Add(user);
                    Client client = new Client { Name = model.FullName, Phone = model.Phone };
                    _context.Clients.Add(client);
                    await _context.SaveChangesAsync();
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.FullName == model.FullName && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация
                    return RedirectToAction("GetAll", "Visits", new {userName =user.FullName  });
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet("Edit")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Id);
            Register model = new Register()
            {
                Id = user.Id,
                FullName = user.FullName,
                Phone = user.Phone,
            };
            return View(model);
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Register model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (user != null)
                {
                    user.Password = model.Password;
                    user.FullName = model.FullName;
                    user.Phone = model.Phone;
                    if (await TryUpdateModelAsync<User>(user,
                        "",
                        c => c.FullName, c => c.Phone, c => c.Password))
                    {
                        try
                        {
                            await _context.SaveChangesAsync();
                            Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                            if (userRole != null)
                                user.Role = userRole;
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            await Authenticate(user);
                        }
                        catch (DbUpdateException /* ex */)
                        {
                            //Log the error (uncomment ex variable name and write a log.)
                            ModelState.AddModelError("", "Unable to save changes. " +
                                "Try again, and if the problem persists, " +
                                "see your system administrator.");
                        }
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.FullName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}