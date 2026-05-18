using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmptyMvcProject.Data;
using EmptyMvcProject.Models;

namespace EmptyMvcProject.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Videos?playlistId=5
        public async Task<IActionResult> Index(int? playlistId)
        {
            if (playlistId == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlists
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(m => m.Id == playlistId);

            if (playlist == null)
            {
                return NotFound();
            }

            ViewData["PlaylistName"] = playlist.Name;
            ViewData["PlaylistId"] = playlist.Id;

            return View(playlist.Videos);
        }

        // POST: Videos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Question,Answer,Url,IsUploaded,PlaylistId")] Video video)
        {
            if (ModelState.IsValid)
            {
                video.Created = DateTime.UtcNow;
                _context.Add(video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { playlistId = video.PlaylistId });
            }

            // If we fail, reload the Index view with the playlist data
            var playlist = await _context.Playlists
                .Include(p => p.Videos)
                .FirstOrDefaultAsync(m => m.Id == video.PlaylistId);

            if (playlist != null) {
                ViewData["PlaylistName"] = playlist.Name;
                ViewData["PlaylistId"] = playlist.Id;
                return View("Index", playlist.Videos);
            }
            return NotFound();
        }

        // GET: Videos/Answer/5
        public async Task<IActionResult> Answer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var video = await _context.Videos
                .Include(v => v.Playlist)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

        // POST: Videos/Answer/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(int id, [Bind("Id,Answer")] Video updatedVideo)
        {
            if (id != updatedVideo.Id)
            {
                return NotFound();
            }

            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    video.Answer = updatedVideo.Answer;
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { playlistId = video.PlaylistId });
            }
            return View(video);
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.Id == id);
        }
    }
}
