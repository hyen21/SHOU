using System;
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
using XAct.Users;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace SHOU.Controllers
{
    public class UsersController : Controller
    {
        private readonly SHOUContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        ProfileEditModel profileEditModel = new ProfileEditModel();
        bool? isDisable;
        public UsersController(SHOUContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;
            
            if (claimsPrincipal.Identity.IsAuthenticated)
            {
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
                        new Claim(ClaimTypes.NameIdentifier, data.UserName),
                        new Claim("Id", data.Id),
                        new Claim("UserName", data.UserName),
                        new Claim("Name", data.Name),
                        new Claim("Avatar", data.Avatar ?? "/Img/noprofil.jpg"),
                    };

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity), authenticationProperties);


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
                        Models.User newUser = new Models.User()
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

        // GET: Users/PersonalPage
        public async Task<IActionResult> PersonalPage()
        {
            List<PostViewModel> postViewModels = new List<PostViewModel>();
            var id = @User.FindFirst("Id")?.Value;
            var listPost = await _context.Posts.Where(c => c.IdUser == id).OrderByDescending(c => c.CreateTime).ToListAsync();
            foreach (var item in listPost)
            {
                PostViewModel postViewModel = new PostViewModel();
                postViewModel.Id = item.Id;
                postViewModel.IdUser = item.IdUser;
                postViewModel.Image = item.Image;
                postViewModel.Content = item.Content;
                postViewModel.Video = item.Video;
                postViewModel.Video = item.Video;
                postViewModel.CreateTime = item.CreateTime;

                var userPost = await _context.Users.FirstOrDefaultAsync(c => c.Id == item.IdUser);
                postViewModel.CountLike = await _context.Likes.Where(c => c.IdPost == item.Id).CountAsync();
                postViewModel.Avatar = userPost?.Avatar ?? "/Img/noprofil.jpg";
                postViewModel.Name = userPost?.Name ?? "Nguyễn Văn A";
                postViewModel.Liked = await _context.Likes.AnyAsync(c => c.IdPost == item.Id && c.IdUser == id);
                postViewModels.Add(postViewModel);
            }
            return View(postViewModels);
        }

        public async Task<JsonResult> GetProfile()
        {
            try
            {
                var id = @User.FindFirst("Id")?.Value;
                var user = await _context.Users.FindAsync(id);
                return Json(new { code = 200, user = user, msg = "Lấy thông tin tài khoản thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Lấy thông tin tài khoản thất bại!" });
            }
        }

        public async Task<JsonResult> UpdateUser(string name, DateTime? birthday, bool? gender, string phone, string email, string addressEdit)
        {
            try
            {
                var id = @User.FindFirst("Id")?.Value;
                var user = await _context.Users.FindAsync(id);
                user.Name = name;
                user.Birthday = birthday;
                user.Address = addressEdit;
                user.Gender = gender;
                user.Phone = phone;
                user.Email = email;
                _context.Update(user);
                await _context.SaveChangesAsync();
                List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                        new Claim("Id", user.Id),
                        new Claim("UserName", user.UserName),
                        new Claim("Name", user.Name),
                        new Claim("Avatar", user.Avatar),
                    };
                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity), authenticationProperties);
                return Json(new { code = 200, msg = "Cập nhật thông tin thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Cập nhật thông tin thất bại!" });
            }
        }

        public async Task<JsonResult> UpdateAvatar(IFormFile avatar)
        {
            try
            {
                var id = @User.FindFirst("Id")?.Value;
                var user = await _context.Users.FindAsync(id);

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Copy tệp tin vào thư mục uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn vào cơ sở dữ liệu
                user.Avatar = "/avatars/" + fileName;

                _context.Update(user);
                await _context.SaveChangesAsync();

                List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserName),
                        new Claim("Id", user.Id),
                        new Claim("UserName", user.UserName),
                        new Claim("Name", user.Name),
                        new Claim("Avatar", user.Avatar),
                    };

                ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity), authenticationProperties);
                return Json(new { code = 200, msg = "Cập nhật avatar thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Cập nhật avatar thất bại!" });
            }
        }

        public async Task<JsonResult> UpdateBackground(IFormFile avatar)
        {
            try
            {
                var id = @User.FindFirst("Id")?.Value;
                var user = await _context.Users.FindAsync(id);

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "backgrounds");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatar.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Copy tệp tin vào thư mục uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                // Lưu đường dẫn vào cơ sở dữ liệu
                user.Background = "/backgrounds/" + fileName;

                _context.Update(user);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, msg = "Cập nhật ảnh bìa thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Cập nhật ảnh bìa thất bại!" });
            }
        }

        public async Task<JsonResult> ChangePassword(string oldPass, string newPass, string reNewPass)
        {
            try
            {
                if (newPass != reNewPass)
                {
                    return Json(new { code = 500, msg = "Nhập lại mật khẩu không khớp!" });
                }
                byte[] tempOldPass = ASCIIEncoding.ASCII.GetBytes(oldPass);
                byte[] hasDataOldPass = new MD5CryptoServiceProvider().ComputeHash(tempOldPass);
                string hasPassOldPass = "";

                foreach (byte item in hasDataOldPass)
                {
                    hasPassOldPass += item;
                }

                var id = @User.FindFirst("Id")?.Value;
                var user = await _context.Users.FindAsync(id);

                if (hasPassOldPass != user.Password)
                {
                    return Json(new { code = 500, msg = "Mật khẩu cũ không đúng!" });
                }

                byte[] tempNewPass = ASCIIEncoding.ASCII.GetBytes(newPass);
                byte[] hasDataNewPass = new MD5CryptoServiceProvider().ComputeHash(tempNewPass);
                string hasPassNewPass = "";

                foreach (byte item in hasDataNewPass)
                {
                    hasPassNewPass += item;
                }
                user.Password = hasPassNewPass;

                _context.Update(user);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, msg = "Đổi mật khẩu thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Đổi mật khẩu thất bại!" });
            }
        }
    }
}
