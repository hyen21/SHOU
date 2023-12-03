using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SHOU.Contexts;
using SHOU.Extentions;
using SHOU.Models;

namespace SHOU.Controllers
{
    public class LikesController : Controller
    {
        private readonly SHOUContext _context;

        public LikesController(SHOUContext context)
        {
            _context = context;
        }

        // GET: Likes
        public async Task<IActionResult> Index()
        {
              return _context.Likes != null ? 
                          View(await _context.Likes.ToListAsync()) :
                          Problem("Entity set 'SHOUContext.Likes'  is null.");
        }

        // GET: Likes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        // GET: Likes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Likes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUser,IdPost")] Like like)
        {
            if (ModelState.IsValid)
            {
                _context.Add(like);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(like);
        }

        // GET: Likes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }
            return View(like);
        }

        // POST: Likes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,IdUser,IdPost")] Like like)
        {
            if (id != like.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(like);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LikeExists(like.Id))
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
            return View(like);
        }

        // GET: Likes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Likes == null)
            {
                return NotFound();
            }

            var like = await _context.Likes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (like == null)
            {
                return NotFound();
            }

            return View(like);
        }

        // POST: Likes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Likes == null)
            {
                return Problem("Entity set 'SHOUContext.Likes'  is null.");
            }
            var like = await _context.Likes.FindAsync(id);
            if (like != null)
            {
                _context.Likes.Remove(like);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LikeExists(string id)
        {
          return (_context.Likes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> LikePost(string idUser, string idPost)
        {
            try
            {
                var like = await _context.Likes.FirstOrDefaultAsync(c => c.IdPost == idPost && c.IdUser == idUser);
                if (like != null)
                {
                    _context.Likes.Remove(like);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    Like like1 = new Like();
                    like1.IdUser = idUser;
                    like1.IdPost = idPost;
                    like1.Id = ObjectExtentions.GenerateGuid();
                    _context.Likes.Add(like1);
                    await _context.SaveChangesAsync();
                }
                return Json(new { code = 200});
            }
            catch (Exception ex)
            {
                return Json(new { code = 500 });
            }
        }

        public async Task<IActionResult> CountLike(string idPost)
        {
            try
            {
                var like = await _context.Likes.CountAsync(c => c.IdPost == idPost);
                return Json(new { code = 200, countLike = like });
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, countLike = 0 });
            }
        }
    }
}
