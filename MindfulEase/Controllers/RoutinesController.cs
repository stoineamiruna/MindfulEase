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
    public IActionResult Index(string search)
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

        var disLikedResources = db.ApplicationUserResources
            .Where(aur => aur.UserId == userId && aur.IsLiked == false)
            .Join(db.Resources,
                aur => aur.ResourceId,
                r => r.Id,
                (aur, r) => r)
            .ToList();
       

        var savedResources = db.SavedResources
                            .Where(aur => aur.UserId == userId && aur.IsSaved == true)
                            .Join(db.Resources,
                                aur => aur.ResourceId,
                                r => r.Id,
                                (aur, r) => r)
                            .ToList();
        if (!string.IsNullOrEmpty(search))
        {


            combinedResources = combinedResources
                .Where(r => r.Title.ToLower().Contains(search.ToLower()) ||
                            db.ResourceTags
                            .Join(db.Tags, tgt => tgt.TagId, t => t.Id, (tgt, t) => new { tgt.ResourceId, t.Title })
                            .Any(joined => joined.ResourceId == r.Id &&
                                          joined.Title.ToLower().Contains(search.ToLower())))
                .ToList();
            savedResources = savedResources
                .Where(r => r.Title.ToLower().Contains(search.ToLower()) ||
                            db.ResourceTags
                            .Join(db.Tags, tgt => tgt.TagId, t => t.Id, (tgt, t) => new { tgt.ResourceId, t.Title })
                            .Any(joined => joined.ResourceId == r.Id &&
                                          joined.Title.ToLower().Contains(search.ToLower())))
                .ToList();
            likedResources = likedResources
                .Where(r => r.Title.ToLower().Contains(search.ToLower()) ||
                            db.ResourceTags
                            .Join(db.Tags, tgt => tgt.TagId, t => t.Id, (tgt, t) => new { tgt.ResourceId, t.Title })
                            .Any(joined => joined.ResourceId == r.Id &&
                                          joined.Title.ToLower().Contains(search.ToLower())))
                .ToList();
            disLikedResources = disLikedResources
                .Where(r => r.Title.ToLower().Contains(search.ToLower()) ||
                            db.ResourceTags
                            .Join(db.Tags, tgt => tgt.TagId, t => t.Id, (tgt, t) => new { tgt.ResourceId, t.Title })
                            .Any(joined => joined.ResourceId == r.Id &&
                                          joined.Title.ToLower().Contains(search.ToLower())))
                .ToList();
        }

        ViewBag.LikedResources = likedResources;
        ViewBag.SavedResources = savedResources.Select(r => r.Id).ToList();
        ViewBag.SavedResourcesList = savedResources;
        ViewBag.DisLikedResources = disLikedResources.Select(r => r.Id).ToList();
        ViewBag.ReallyLikedResources = likedResources.Select(r => r.Id).ToList();
        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }
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

    public IActionResult SeeMore(string type, string search)
    {
        string section = type;
        SetAccessRights();
        string userId = _userManager.GetUserId(User);

        IQueryable<Resource> resources;
        Console.WriteLine(section);

        if (section == "liked")
        {
            resources = db.ApplicationUserResources
                          .Where(aur => aur.UserId == userId && aur.IsLiked == true)
                          .Join(db.Resources, aur => aur.ResourceId, r => r.Id, (aur, r) => r);
        }
        else if (section == "saved")
        {
            resources = db.SavedResources
                          .Where(aur => aur.UserId == userId && aur.IsSaved == true)
                          .Join(db.Resources, aur => aur.ResourceId, r => r.Id, (aur, r) => r);
        }
        else if (!string.IsNullOrEmpty(section))
        {
            resources = db.Resources.Where(r => r.ResourceType == section);
        }
        else
        {
            resources = db.Resources;
        }

        // Obțin resursele apreciate și salvate (dar fără `ToList()`)
        var likedResources = db.ApplicationUserResources
                                .Where(aur => aur.UserId == userId && aur.IsLiked == true)
                                .Join(db.Resources, aur => aur.ResourceId, r => r.Id, (aur, r) => r);

        var disLikedResources = db.ApplicationUserResources
                                .Where(aur => aur.UserId == userId && aur.IsLiked == false)
                                .Join(db.Resources, aur => aur.ResourceId, r => r.Id, (aur, r) => r);

        var savedResourceIds = db.SavedResources
                                .Where(sr => sr.UserId == userId && sr.IsSaved == true)
                                .Select(sr => sr.ResourceId);

        // Dacă există căutare, aplicăm filtrarea
        if (!string.IsNullOrEmpty(search))
        {
            string searchLower = search.ToLower(); // Aplică `ToLower()` doar la căutare

            resources = resources.Where(r =>
                r.Title.ToLower().Contains(searchLower) ||
                db.ResourceTags.Any(rt => rt.ResourceId == r.Id && rt.Tag.Title.ToLower().Contains(searchLower))
            );

            likedResources = likedResources.Where(r =>
                r.Title.ToLower().Contains(searchLower) ||
                db.ResourceTags.Any(rt => rt.ResourceId == r.Id && rt.Tag.Title.ToLower().Contains(searchLower))
            );

            disLikedResources = disLikedResources.Where(r =>
                r.Title.ToLower().Contains(searchLower) ||
                db.ResourceTags.Any(rt => rt.ResourceId == r.Id && rt.Tag.Title.ToLower().Contains(searchLower))
            );
        }

        // Convertim listele finale
        ViewBag.DisLikedResources = disLikedResources.Select(r => r.Id).ToList();
        ViewBag.ReallyLikedResources = likedResources.Select(r => r.Id).ToList();
        ViewBag.SavedResources = savedResourceIds.ToList(); // Folosim ID-uri pentru resurse salvate
        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }
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
