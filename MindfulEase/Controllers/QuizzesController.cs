using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulEase.Data;
using MindfulEase.Models;
using System;
using System.Linq;

namespace MindfulEase.Controllers
{
    
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        public QuizzesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.EsteModerator = User.IsInRole("Moderator");
        }

        // Afișează formularul pentru crearea unui nou quiz
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult New()
        {
            return View();
        }

        // Adaugă quiz-ul în baza de date
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Quizzes.Add(quiz);
                db.SaveChanges();

                TempData["message"] = "Quiz created successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction("Show", new { id = quiz.Id }); // Redirectează către pagina de detalii a quiz-ului
            }

            // În caz de eroare, rămâne pe aceeași pagină
            TempData["message"] = "There was an error creating the quiz.";
            TempData["messageType"] = "alert-danger";
            return View(quiz);
        }

        // GET: Edit an existing quiz
        [Authorize(Roles = "Admin, Moderator")]
        public IActionResult Edit(int id)
        {
            var quiz = db.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                TempData["message"] = "Quiz not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            return View(quiz);
        }

        // POST: Update quiz details
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public IActionResult Edit(int id, Quiz updatedQuiz)
        {
            var quiz = db.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                TempData["message"] = "Quiz not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            quiz.Title = updatedQuiz.Title;
            quiz.Description = updatedQuiz.Description;
            quiz.CategoryMapping = updatedQuiz.CategoryMapping;
            quiz.Result = updatedQuiz.Result;

            db.SaveChanges();

            TempData["message"] = "Quiz updated successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { id = quiz.Id }); // Redirecționează către pagina de detalii
        }

        // Afișează detalii pentru un quiz specific
        [Authorize(Roles = "Admin, Moderator, User")]
        public IActionResult Show(int id)
        {
            SetAccessRights();
            var quiz = db.Quizzes.Include(q => q.Questions).FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                TempData["message"] = "Quiz not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            quiz.Questions = quiz.Questions.OrderBy(q => q.Order).ToList();
            // Returnează quiz-ul cu întrebările sale
            return View(quiz);
        }

        // POST: Adaugă o întrebare nouă
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Show(int id, string questionText, int order, bool isReversed)
        {
            SetAccessRights();
            var quiz = db.Quizzes.Include(q => q.Questions).FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                TempData["message"] = "Quiz not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
            Console.WriteLine(isReversed);
            // Crează o întrebare nouă
            var newQuestion = new QuestionQuiz
            {
                Text = questionText,
                Order = order,
                IsReversed = isReversed,
                QuizId = quiz.Id
            };

            db.QuestionQuizzes.Add(newQuestion);
            db.SaveChanges();

            TempData["message"] = "Question added successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Show", new { id = quiz.Id });
        }

        // POST: Delete a quiz
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var quiz = db.Quizzes
                 .Include(q => q.Questions)
                 .Include(q => q.Users) // Include related ApplicationUser entries
                 .FirstOrDefault(q => q.Id == id);

            if (User.IsInRole("Admin") == false && User.IsInRole("Moderator") == false)
            {
                TempData["message"] = "Access denied.";
                TempData["messageType"] = "alert-danger";
                // Redirecționează înapoi la pagina anterioară
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer);
                }
            }
            if (quiz == null)
            {
                TempData["message"] = "Quiz not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show");
            }
            // Remove the quiz-question relationships
            foreach (var question in quiz.Questions)
            {
                db.ApplicationUserQuestionQuizzes.RemoveRange(question.UserQuestionQuizzes); // Remove user-question relationships
            }

            // Remove the quiz-user relationships
            db.ApplicationUserQuizzes.RemoveRange(quiz.Users); // Remove the users associated with the quiz

            // Remove all associated questions
            db.QuestionQuizzes.RemoveRange(quiz.Questions);

            // Remove the quiz itself
            db.Quizzes.Remove(quiz);

            // Save changes to the database
            db.SaveChanges();

            TempData["message"] = "Quiz deleted successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction("Index");
        }

        // POST: Delete a question
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult DeleteQuestion(int id)
        {
            var question = db.QuestionQuizzes.FirstOrDefault(q => q.Id == id);
            if (question == null)
            {
                TempData["message"] = "Question not found.";
                TempData["messageType"] = "alert-danger";
                // Redirecționează înapoi la pagina anterioară
        
                var refererr = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(refererr))
                {
                    return Redirect(refererr);
                }
            }
            if (question.UserQuestionQuizzes != null)
            {
                db.ApplicationUserQuestionQuizzes.RemoveRange(question.UserQuestionQuizzes);
            }
            db.QuestionQuizzes.Remove(question);
            db.SaveChanges();

            TempData["message"] = "Question deleted successfully!";
            TempData["messageType"] = "alert-success";

            // Redirecționează înapoi la pagina anterioară
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction("Index");
        }

        // POST: Update a question
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        public ActionResult EditQuestion(int id, [FromForm] QuestionQuiz updatedQuestion)
        {
            var question = db.QuestionQuizzes.FirstOrDefault(q => q.Id == id);
            if (question == null)
            {
                TempData["message"] = "Question not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", new { id = question.QuizId });
            }
            Console.WriteLine("updatedQuestion.IsReversed");
            Console.WriteLine(updatedQuestion.IsReversed);
            // Actualizăm câmpurile întrebării cu noile valori
            question.Text = updatedQuestion.Text;
            question.IsReversed = updatedQuestion.IsReversed;
            question.Order =   updatedQuestion.Order;

            // Salvăm modificările în baza de date
            db.SaveChanges();

            TempData["message"] = "Question updated successfully!";
            TempData["messageType"] = "alert-success";

            // Redirecționăm către pagina de detalii a quiz-ului
            return RedirectToAction("Show", new { id = question.QuizId });
        }

        [Authorize(Roles = "Admin, Moderator, User")]
        public ActionResult Solve(int id)
        {
            var quiz = db.Quizzes.Include("Questions").FirstOrDefault(q => q.Id == id);
            if (quiz == null) {
                TempData["message"] = "Question not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", id );
            }

            ViewBag.Title = quiz.Title;
            ViewBag.Description = quiz.Description;
            ViewBag.CategoryMapping = quiz.CategoryMapping;

            return View(quiz);
        }
        [Authorize(Roles = "Admin, Moderator, User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(int quizId, Dictionary<int, int> answers)
        {
            var userId = _userManager.GetUserId(User);
            var quiz = db.Quizzes.Include("Questions").FirstOrDefault(q => q.Id == quizId);
            if (quiz == null)
            {
                TempData["message"] = "Question not found.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Show", new { id = quizId });
            }

            var responseEntries = new List<ApplicationUserQuestionQuiz>();
            var categoryScores = new Dictionary<string, int>();

            var categoryMapping = quiz.CategoryMapping
                .Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Split(':'))
                .Where(pair => pair.Length == 2)
                .ToDictionary(
                    pair => pair[0].Trim(),
                    pair => pair[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(num => int.TryParse(num, out int val) ? val : -1)
                                  .Where(num => num != -1)
                                  .ToList()
                );

            int totalScore = 0;

            foreach (var answer in answers)
            {
                var question = quiz.Questions.OrderBy(q => q.Order).FirstOrDefault(q => q.Id == answer.Key);
                if (question == null) continue;

                int score = question.IsReversed ? 4 - answer.Value : answer.Value;
                totalScore += score;

                // Asocierea întrebării la categorie pe baza ordinii întrebărilor
                int questionIndex = quiz.Questions.OrderBy(q => q.Order).ToList().FindIndex(q => q.Id == question.Id);

                foreach (var category in categoryMapping)
                {
                    // Aici alegi categoria pe baza ordinii întrebărilor
                    if (category.Value.Contains(questionIndex + 1)) // +1 pentru a începe de la 1, nu de la 0
                    {
                        if (!categoryScores.ContainsKey(category.Key))
                            categoryScores[category.Key] = 0;

                        categoryScores[category.Key] += score;
                    }
                }

                // Verifică dacă există deja o înregistrare pentru această întrebare și utilizator
                var existingResponse = db.ApplicationUserQuestionQuizzes
                    .FirstOrDefault(r => r.UserId == userId && r.QuestionId == answer.Key);

                if (existingResponse != null)
                {
                    // Actualizează scorul dacă există deja un răspuns
                    existingResponse.ResponseValue = score;
                }
                else
                {
                    // Creează o nouă înregistrare dacă nu există
                    responseEntries.Add(new ApplicationUserQuestionQuiz
                    {
                        UserId = userId,
                        QuestionId = answer.Key,
                        ResponseValue = score
                    });
                }
            }


            // Verifică dacă există deja un răspuns completat pentru acest quiz
            var existingQuizResponse = db.ApplicationUserQuizzes
                .FirstOrDefault(r => r.UserId == userId && r.QuizId == quizId);

            if (existingQuizResponse != null)
            {
                // Actualizează scorul și starea pentru quiz-ul existent
                existingQuizResponse.TotalScore = totalScore;
                existingQuizResponse.IsCompleted = true;
            }
            else
            {
                // Creează o nouă înregistrare pentru quiz-ul completat
                db.ApplicationUserQuizzes.Add(new ApplicationUserQuiz
                {
                    UserId = userId,
                    QuizId = quizId,
                    TotalScore = totalScore,
                    IsCompleted = true
                });
            }

            // Adaugă doar înregistrările noi de răspunsuri în baza de date
            if (responseEntries.Any())
            {
                db.ApplicationUserQuestionQuizzes.AddRange(responseEntries);
            }

            db.SaveChanges();
            Console.WriteLine(totalScore);
            Console.WriteLine(categoryScores);
            ViewBag.TotalScore = totalScore;
            ViewBag.CategoryScores = categoryScores;

            return View("Submit", quiz);
        }


    }
}
