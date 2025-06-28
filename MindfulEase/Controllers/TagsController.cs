using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MindfulEase.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View(await _context.Tags.ToListAsync());
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]       
        public async Task<IActionResult> Create(Tag tag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
                TempData["message"] = "Tag created successfully!";
                TempData["messageType"] = "alert-info";
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = "There was an error creating the tag.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Tag tag)
        {
            if (id != tag.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(tag);
                await _context.SaveChangesAsync();
                TempData["message"] = "Tag modified successfully!";
                TempData["messageType"] = "alert-info";
                return RedirectToAction(nameof(Index));
            }
            TempData["message"] = "There was an error editing the tag.";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound();

            var resourceTags = _context.ResourceTags.Where(sr => sr.TagId == id).ToList();
            _context.ResourceTags.RemoveRange(resourceTags);

            var quizTags = _context.QuizTags.Where(sr => sr.TagId == id).ToList();
            _context.QuizTags.RemoveRange(quizTags);

            var therapeuticGameTags = _context.TherapeuticGameTags.Where(sr => sr.TagId == id).ToList();
            _context.TherapeuticGameTags.RemoveRange(therapeuticGameTags);

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            TempData["message"] = "Tag deleted successfully!";
            TempData["messageType"] = "alert-info";
            return RedirectToAction(nameof(Index));
        }
    }
}
