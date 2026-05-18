using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmptyMvcProject.Data;
using EmptyMvcProject.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace EmptyMvcProject.Controllers
{
    public class VideosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public VideosController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
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
        public async Task<IActionResult> Create([Bind("Name,Answer,Url,Description,IsUploaded,PlaylistId,QuestionImage")] Video video)
        {
            if (ModelState.IsValid)
            {
                if (video.QuestionImage != null && video.QuestionImage.Length > 0)
                {
                    // Ensure wwwroot path exists
                    string webRootPath = _hostEnvironment.WebRootPath;
                    if (string.IsNullOrWhiteSpace(webRootPath))
                    {
                        webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    string uploadsFolder = Path.Combine(webRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Sanitize file name
                    string fileExtension = Path.GetExtension(video.QuestionImage.FileName);
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await video.QuestionImage.CopyToAsync(fileStream);
                    }

                    // Save file path to database
                    video.Question = "/uploads/" + uniqueFileName;
                }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadQuestionImage(int id, IFormFile questionImage)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            if (questionImage != null && questionImage.Length > 0)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                string uploadsFolder = Path.Combine(webRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileExtension = Path.GetExtension(questionImage.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await questionImage.CopyToAsync(fileStream);
                }

                video.Question = "/uploads/" + uniqueFileName;
                _context.Update(video);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index), new { playlistId = video.PlaylistId });
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
