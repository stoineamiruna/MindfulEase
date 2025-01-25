using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MindfulEase.Data;
using MindfulEase.Models;
using MindfulEase.Services;

public class RoutinesController : Controller
{
    private readonly ApplicationDbContext db;
    private readonly RecommendationService _recommendationService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoutinesController(ApplicationDbContext context,
        RecommendationService recommendationService,
        UserManager<ApplicationUser> userManager)
    {
        _recommendationService = recommendationService;
        _userManager = userManager;
        db = context;
    }

    public IActionResult Index()
    {
        string userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest("User ID is required.");
        }

        var user = db.ApplicationUsers.Find(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Dacă utilizatorul nu are ClusterId, îi asignăm unul
        if (user.ClusterId == null)
        {
            var clusteringService = new ClusteringService(db);
            clusteringService.AssignClustersToUsers();
            user = db.ApplicationUsers.Find(userId); // Reîncarcă utilizatorul cu ClusterId actualizat
        }

        var recommendedResources = _recommendationService.GetRecommendedResources(userId);
        return View(recommendedResources);
    }

    [HttpPost]
    public IActionResult ProvideFeedback(int resourceId, bool isLiked)
    {
        string userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId) || resourceId <= 0)
        {
            return BadRequest("Invalid user ID or resource ID.");
        }

        try
        {
            _recommendationService.RecordFeedback(userId, resourceId, isLiked);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
