using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHOU.Contexts;
using SHOU.Extentions;
using SHOU.Models;
using SHOU.Models.EditModel;
using XSystem.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using XAct.Users;

namespace SHOU.Controllers
{
    public class PostsController : Controller
    {
        private readonly SHOUContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(SHOUContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile img, string textPost)
        {
            try
            {

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgPost");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Copy tệp tin vào thư mục uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }

                Post post = new Post();
                var id = @User.FindFirst("Id")?.Value;
                post.Id = ObjectExtentions.GenerateGuid();
                post.IdUser = id;
                post.Content = textPost;
                post.CreateTime = DateTime.Now;

                // Lưu đường dẫn vào cơ sở dữ liệu
                post.Image = "/imgPost/" + fileName;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, msg = "thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "thất bại!" });
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post != null)
                {
                    _context.Posts.Remove(post);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Json(new { code = 500, msg = "Xóa thất bại" });
                }
                return Json(new { code = 200, msg = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Xóa thất bại" });
            }
        }

        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                var post = await _context.Posts.FindAsync(id);
                if (post != null)
                {
                    return Json(new { code = 200, detail = post, msg = "lấy dữ liệu thành công" });
                }
                else
                {
                    return Json(new { code = 500, msg = "lấy dữ liệu thất bại" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "lấy dữ liệu thất bại" });
            }
        }

        public async Task<IActionResult> Update(IFormFile img, string textPost, string idPost)
        {
            try
            {
                var post = await _context.Posts.FindAsync(idPost);
                if (post != null)
                {
                    if (img != null)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgPost");
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Copy tệp tin vào thư mục uploads
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(fileStream);
                        }

                        // Lưu đường dẫn vào cơ sở dữ liệu
                        post.Image = "/imgPost/" + fileName;
                    }
                    post.Content = textPost;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                return Json(new { code = 200, msg = "thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "thất bại!" });
            }
        }
    }
}
