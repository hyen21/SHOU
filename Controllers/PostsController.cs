using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHOU.Contexts;
using SHOU.Models;
using SHOU.Models.EditModel;
using static SHOU.Models.EditModel.PostEditModel;
using SHOU.Extentions;
using XAct.Users;
using Nest;
using DocumentFormat.OpenXml.Wordprocessing;
using Grpc.Core;

namespace SHOU.Controllers
{
    public class PostsController : Controller
    {
        private readonly SHOUContext _context;

        public PostsController(SHOUContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.User;
            var name = user.FindFirst("Name")?.Value;
            ViewData["Name"] = name;
            return _context.Posts != null ?
                        View(await _context.Posts.ToListAsync()) :
                        Problem("Entity set 'SHOUContext.Posts'  is null.");
        }
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Users");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            var user = HttpContext.User;
            var idUser = user.FindFirst("Id")?.Value;

            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewPost post, IFormFile? image)
        {
            //lấy ra User đang sử dụng
            var user = HttpContext.User;
            var idUser = user.FindFirst("Id")?.Value;
            if (post.IdUser == null)
            {
                post.IdUser = idUser;
            }

            //xử lý hình ảnh
            if (image != null && image.Length > 0)
            {
                post.Image = image.FileName;
                var fileName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "URLImage", fileName);
                //post.Image = filePath;
                using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileSrteam);
                }
            }


                if (ModelState.IsValid)
                {
                    var newPost = new Post()
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        IdUser = post.IdUser,
                        Content = post.Content,
                        Image = post.Image,
                        CreateAt = DateTime.Now

                    };
                    _context.Add(newPost);
                    await _context.SaveChangesAsync();
                    
                   
                    return RedirectToAction(nameof(Index));
                }
            
            return View(post);
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, NewPost post, IFormFile? image)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (image != null && image.ToString() != post.Image)
            {
                post.Image = image.FileName;
                var fileName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "URLImage", fileName);

                using (var fileSrteam = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileSrteam);
                }
            }


                if (ModelState.IsValid)
                {
                    
                        var updatePost = new Post()
                        {
                            Id = post.Id,
                            IdUser = post.IdUser,
                            Content = post.Content,
                            Image = post.Image,
                            CreateAt = DateTime.Now

                        };
                        _context.Update(updatePost);
                        await _context.SaveChangesAsync();
                       
                        return RedirectToAction(nameof(Index));
                    }
                return RedirectToAction(nameof(Index));
            }
        

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'SHOUContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(string id)
        {
            return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
