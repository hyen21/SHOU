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
                var id = @User.FindFirst("Id")?.Value;

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imgPost");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Copy tệp tin vào thư mục uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }

                Post post = new Post();
                post.Id = ObjectExtentions.GenerateGuid();
                post.IdUser = id;
                post.Content = textPost;
                post.CreateTime = DateTime.Now;
                // Lưu đường dẫn vào cơ sở dữ liệu
                post.Image = "/imgPost/" + fileName;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return Json(new { code = 200, msg = "Tạo bài viết thành công!" });

            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Tạo bài viết thất bại!" });
            }
        }
    }
}
