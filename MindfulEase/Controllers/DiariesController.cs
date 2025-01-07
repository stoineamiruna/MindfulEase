using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;

namespace MindfulEase.Controllers
{
    public class DiariesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiariesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        // Show a single diary entry
        public IActionResult Show(int id)
        {
            var diary = db.Diaries.Include(d => d.User).FirstOrDefault(d => d.Id == id);
            if (diary == null)
            {
                TempData["message"] = "Diary entry not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // Display the create diary form
        public IActionResult New()
        {
            var userId = _userManager.GetUserId(User); 
            var userDiaries = db.Diaries.Where(d => d.UserId == userId).ToList(); 
            ViewBag.UserDiaries = userDiaries; 
            return View();
        }

        // Create a new diary entry
        [HttpPost]
        public IActionResult New(Diary newDiary)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                newDiary.UserId = userId;
                newDiary.Content = newDiary.Content;
                newDiary.EntryDate = newDiary.EntryDate; 
                Console.WriteLine($"Content: {newDiary.Content}, EntryDate: {newDiary.EntryDate}");
                db.Diaries.Add(newDiary);
                db.SaveChanges();

                TempData["message"] = "Diary entry created successfully!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("New");
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            return View(newDiary);
        }

        // Display the edit form for a diary entry
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to edit it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // Save the changes to a diary entry
        [HttpPost]
        public IActionResult Edit(int id, Diary updatedDiary)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to edit it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("New");
            }

            diary.Content = updatedDiary.Content; // Update the content of the diary
            diary.EntryDate =  updatedDiary.EntryDate;

            db.SaveChanges();

            TempData["message"] = "Diary entry updated successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var diary = db.Diaries.FirstOrDefault(d => d.Id == id);
            if (diary == null || diary.UserId != _userManager.GetUserId(User))
            {
                TempData["message"] = "Diary entry not found or you do not have permission to delete it.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("New");
            }

            db.Diaries.Remove(diary);
            db.SaveChanges();

            TempData["message"] = "Diary entry deleted successfully!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("New");
        }
    }
}
