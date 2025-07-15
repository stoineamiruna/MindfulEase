using MindfulEase.Data;
using MindfulEase.Models;
using Microsoft.EntityFrameworkCore;
using Humanizer.Localisation;

public class RecommendationService
{
    private readonly ApplicationDbContext _dbContext;

    public RecommendationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Resource> GetRecommendedResources(string userId)
    {
     
        var user = _dbContext.ApplicationUsers.Find(userId);
        if (user == null || user.ClusterId == null) return new List<Resource>();

        var clusterRoutine = _dbContext.Routines
            .Include(r => r.Resources) // Includem relatia cu resursele
            .FirstOrDefault(cr => cr.ClusterId == user.ClusterId);

        return clusterRoutine?.Resources ?? new List<Resource>();
    }

    public void RecordFeedback(string userId, int resourceId, bool isLiked)
    {
        if (_dbContext.ApplicationUsers.Find(userId) == null || !_dbContext.Resources.Any(r => r.Id == resourceId))
        {
            throw new ArgumentException("Invalid user or resource ID");
        }

        // Verificam daca relatia exista deja
        var existingUserResource = _dbContext.ApplicationUserResources
            .FirstOrDefault(ur => ur.UserId == userId && ur.ResourceId == resourceId);

        if (existingUserResource != null)
        {
            existingUserResource.IsLiked = isLiked;
        }
        else
        {
            var userResource = new ApplicationUserResource
            {
                UserId = userId,
                ResourceId = resourceId,
                IsLiked = isLiked
            };

            _dbContext.ApplicationUserResources.Add(userResource);
        }

        _dbContext.SaveChanges();

        // Dupa ce s-a inregistrat feedback-ul, actualizam rutina sau o cream daca nu exista
        UpdateRoutineForCluster(userId, resourceId);
    }

    private void UpdateRoutineForCluster(string userId, int resourceId)
    {
        // Obtinem ClusterId-ul utilizatorului
        var user = _dbContext.ApplicationUsers.Find(userId);
        if (user == null || user.ClusterId == null) return;

        int clusterId = (int)user.ClusterId;

        // Cautam rutina asociata acestui ClusterId
        var routine = _dbContext.Routines
            .Include(r => r.Resources)
            .FirstOrDefault(r => r.ClusterId == clusterId);

        // Obtinem utilizatorii care au apreciat aceasta resursa (like)
        var usersThatLikedResource = _dbContext.ApplicationUserResources
            .Where(ur => ur.ResourceId == resourceId && ur.IsLiked == true)
            .Select(ur => ur.UserId)
            .ToList();

        // Daca utilizatorii au apreciat resursa, cream sau actualizam rutina
        if (usersThatLikedResource.Any())
        {
            if (routine != null)
            {
                // Adaugam resursa in rutina respectiva, daca nu exista deja
                var resource = _dbContext.Resources.Find(resourceId);
                if (resource != null && !routine.Resources.Contains(resource))
                {
                    routine.Resources.Add(resource);
                }
            }
            else
            {
                // Daca rutina nu exista, o cream si o asociem cu ClusterId-ul
                routine = new Routine
                {
                    ClusterId = clusterId,
                    RoutineDescription = $"Routine for Cluster {clusterId}",
                    Resources = new List<Resource>
                {
                    _dbContext.Resources.Find(resourceId)
                }
                };

                _dbContext.Routines.Add(routine);
            }

            // Salvam modificarile în baza de date
            _dbContext.SaveChanges();
        }
    }

}
