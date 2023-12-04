using Microsoft.AspNetCore.Mvc;
using SHOU.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using XAct.Users;
using SHOU.Models.EditModel;
using SHOU.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SHOU.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SHOUContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(SHOUContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            List<PostViewModel> postViewModels = new List<PostViewModel>();
            var id = @User.FindFirst("Id")?.Value;
            var listPost = await _context.Posts.OrderByDescending(c => c.CreateTime).ToListAsync();
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

        public IActionResult Privacy()
        {
            return View();
        } 

        public async Task<IActionResult>  LogOut()
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
    }
}