﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHOU.Contexts;
using SHOU.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SHOU.Models.EditModel;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using SHOU.Extentions;
using System.Text;
using XSystem.Security.Cryptography;

namespace SHOU.Controllers
{
    public class UsersController : Controller
    {
        private readonly SHOUContext _context;

        public UsersController(SHOUContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'SHOUContext.Users'  is null.");
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,Email,Phone,Address,Avatar,Birthday,Gender")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                return View(user);
                throw;
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Password,Email,Phone,Address,Avatar,Birthday,Gender")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'SHOUContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                ViewData["vietanh123"] = "Your Title";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginEditModel user)
        {
            try
            {
                // mã hóa mật khẩu
                byte[] temp = ASCIIEncoding.ASCII.GetBytes(user.Password);
                byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);
                string hasPass = "";

                foreach (byte item in hasData)
                {
                    hasPass += item;
                }

                // lấy tài khoản đăng nhập
                var data = await _context.Users.Where(c => c.UserName == user.UserName && c.Password == hasPass).FirstOrDefaultAsync();

                //Kiểm tra xem tài khoản có trong database ko
                if (data != null)
                {
                    // thêm các thông tin của user để lưu thông giữ các màn
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                        new Claim("Id", data.Id),
                        new Claim("Name", user.UserName),
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity), authenticationProperties);

                    ViewData["vietanh123"] = "Your Title";

                    // chuyển sang màn trang chủ
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // nếu ko có tài khoản thì ra thông báo sai tk mk
                    ViewData["ValidateMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View();
                throw;
            }
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserSignUpEditModel user)
        {
            try
            {
                bool create = true;
                var regex = new Regex(@"^[a-zA-Z0-9]+$"); // Chỉ có chữ cái và số
                if (!regex.IsMatch(user.UserName)) // Username có đúng vs form trên ko
                {
                    ModelState.AddModelError("UserName", "Tên đăng nhập chỉ chứa chữ cái và số");
                }
                if (user.Password != user.RePassword) // so sánh xem mk vs nhập lại mk có khớp ko
                {
                    ModelState.AddModelError("RePassword", "Mật khẩu nhập lại không khớp.");
                }

                // check trùng tên đăng nhập
                var data = await _context.Users.Where(c => c.UserName == user.UserName).FirstOrDefaultAsync();
                if (data != null)
                {
                    ViewData["SignUpResult"] = "Tên đăng nhập đã tồn tại!";
                }
                else
                {
                    // mã hóa mật khẩu
                    byte[] temp = ASCIIEncoding.ASCII.GetBytes(user.Password);
                    byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);
                    string hasPass = "";

                    foreach (byte item in hasData)
                    {
                        hasPass += item;
                    }
                    if (ModelState.IsValid)
                    {
                        User newUser = new User()
                        {
                            Id = ObjectExtentions.GenerateGuid(),
                            UserName = user.UserName,
                            Password = hasPass,
                            Name = user.UserName
                        };
                        _context.Add(newUser);
                        var result = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            ViewData["SignUpResult"] = "Đăng ký thành công!";
                        }
                        else
                        {
                            ViewData["SignUpResult"] = "Đăng ký không thành công!";
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
                throw;
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> EditProfile(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var profileEditModel = new ProfileEditModel();
            profileEditModel.Birthday = user.Birthday;
            profileEditModel.Address = user.Address;
            profileEditModel.Email = user.Email;
            profileEditModel.Gender = user.Gender;
            profileEditModel.Phone = user.Phone;
            profileEditModel.Avatar = user.Avatar;
            profileEditModel.Background = user.Background;
            profileEditModel.Id = user.Id;
            profileEditModel.Name = user.Name;
            profileEditModel.UserName = user.UserName;

            return View(profileEditModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string id, [Bind("Id,Name,Password,Email,Phone,Address,Avatar,Birthday,Gender")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
    }
}
