using Microsoft.AspNetCore.Authorization;
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
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoutinesController(ApplicationDbContext context,
        RecommendationService recommendationService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
    {
        _recommendationService = recommendationService;
        _userManager = userManager;
        db = context;
        _roleManager = roleManager;
    }
    // Conditiile de afisare a butoanelor de editare si stergere
    private void SetAccessRights()
    {
        ViewBag.AfisareButoane = false;

        ViewBag.EsteAdmin = User.IsInRole("Admin");

        ViewBag.EsteModerator = User.IsInRole("Moderator");

        ViewBag.UserCurent = _userManager.GetUserId(User);
    }

    [Authorize(Roles = "Admin,Moderator,User")]
    public IActionResult Index()
    {
        SetAccessRights();
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
        var allResources = db.Resources.ToList();

        // Creăm o listă combinată cu resursele recomandate urmate de celelalte resurse
        var combinedResources = new List<Resource>();

        // Adăugăm resursele recomandate
        combinedResources.AddRange(recommendedResources);

        // Adăugăm restul resurselor care nu sunt deja în lista de recomandări
        combinedResources.AddRange(allResources.Where(r => !recommendedResources.Contains(r)));

        var likedResources = db.ApplicationUserResources
                            .Where(aur => aur.UserId == userId && aur.IsLiked == true)
                            .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r)
                            .ToList();
        ViewBag.LikedResources = likedResources;

        var disLikedResources = db.ApplicationUserResources
            .Where(aur => aur.UserId == userId && aur.IsLiked == false)
            .Join(db.Resources,
                aur => aur.ResourceId,
                r => r.Id,
                (aur, r) => r)
            .ToList();
        ViewBag.DisLikedResources = disLikedResources.Select(r => r.Id).ToList();  
        ViewBag.ReallyLikedResources = likedResources.Select(r => r.Id).ToList();

        var savedResources = db.SavedResources
                            .Where(aur => aur.UserId == userId && aur.IsSaved == true)
                            .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r)
                            .ToList();

        ViewBag.SavedResources = savedResources.Select(r => r.Id).ToList();
        ViewBag.SavedResourcesList = savedResources;

        Console.WriteLine(User.IsInRole("Admin"));
        return View(combinedResources);
    }

    [Authorize(Roles = "Admin,Moderator,User")]
    [HttpPost]
    public IActionResult ProvideFeedback(int resourceId, bool isLiked)
    {
        SetAccessRights();
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

    public IActionResult SeeMore(string section)
    {
        SetAccessRights();
        string userId = _userManager.GetUserId(User);


        IQueryable<Resource> resources;
        Console.WriteLine(section);
        if (section == "liked")
        {
            // Resurse apreciate de utilizatorul curent
            resources = db.ApplicationUserResources
                          .Where(aur => aur.UserId == userId && aur.IsLiked == true)
                          .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r);
            Console.WriteLine("section: 1");
        }
        else if (section == "saved")
        {
            // Resurse salvate de utilizatorul curent
            resources = db.SavedResources
                          .Where(aur => aur.UserId == userId && aur.IsSaved == true)
                          .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r);
        }
        else if (!string.IsNullOrEmpty(section))
        {
            // Resursele cu tipul specificat
            resources = db.Resources.Where(r => r.ResourceType == section);
            Console.WriteLine("section: 2");
        }
        else
        {
            // Dacă secțiunea nu este specificată, returnează toate resursele
            resources = db.Resources;
            Console.WriteLine("section: 3");
        }
        var likedResources = db.ApplicationUserResources
                            .Where(aur => aur.UserId == userId && aur.IsLiked == true)
                            .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r)
                            .ToList();

        var disLikedResources = db.ApplicationUserResources
            .Where(aur => aur.UserId == userId && aur.IsLiked == false)
            .Join(db.Resources,
                aur => aur.ResourceId,
                r => r.Id,
                (aur, r) => r)
            .ToList();
        ViewBag.DisLikedResources = disLikedResources.Select(r => r.Id).ToList();
        ViewBag.ReallyLikedResources = likedResources.Select(r => r.Id).ToList();

        var savedResources = db.SavedResources
                            .Where(sr => sr.UserId == userId && sr.IsSaved == true)
                            .Select(sr => sr.ResourceId)
                            .ToList();

        ViewBag.SavedResources = savedResources;

        return View(resources.ToList());
    }

    [Authorize(Roles = "Admin,Moderator,User")]
    [HttpPost]
    public IActionResult SaveResource(int resourceId)
    {
        SetAccessRights();
        string userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId) || resourceId <= 0)
        {
            return BadRequest("Invalid user ID or resource ID.");
        }

        try
        {
            // Verificăm dacă utilizatorul a salvat deja această resursă
            var existingEntry = db.SavedResources
                .FirstOrDefault(sr => sr.UserId == userId && sr.ResourceId == resourceId);

            if (existingEntry == null)
            {
                // Adăugăm o nouă intrare dacă nu există
                var savedResource = new SavedResource
                {
                    UserId = userId,
                    ResourceId = resourceId,
                    IsSaved = true,
                    Date = DateTime.UtcNow
                };

                db.SavedResources.Add(savedResource);
                db.SaveChanges();
            }
            else
            {
                existingEntry.IsSaved = !existingEntry.IsSaved;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}
