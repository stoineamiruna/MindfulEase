using MindfulEase.Data;
using MindfulEase.Models;

namespace MindfulEase.Services
{
    public class ChallengeNotificationsService
    {
        private readonly ApplicationDbContext _context;

        public ChallengeNotificationsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CheckActiveChallengesAndSendNotifications()
        {
            // Obține provocările active din baza de date
            var challenges = _context.WeeklyChallenges
                .ToList();

            foreach (var challenge in challenges)
            {
                var users = _context.Users.ToList();

                foreach (var user in users)
                {
                    // Notificare la începutul provocării
                    var startNotificationExists = _context.Notifications.Any(n =>
                        (n.UserId == user.Id ||
                        n.UserId == null) &&
                        n.Link == "/WeeklyChallenges/Index" &&
                        n.Message == $"Challenge {challenge.Title} has started!");

                    if (!startNotificationExists && challenge.StartDate <= DateTime.Now && challenge.EndDate >= DateTime.Now)
                    {
                        var startNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} has started!",
                            Link = "/WeeklyChallenges/Index",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(startNotification);
                        
                    }

                    // Notificare spre sfârșitul provocării
                    var endingSoonNotificationExists = _context.Notifications.Any(n =>
                        n.UserId == user.Id &&
                        n.Link == "/WeeklyChallenges/Index" &&
                        n.Message == $"Challenge {challenge.Title} is ending soon!");

                    if (!endingSoonNotificationExists && (DateTime.Now > challenge.StartDate && DateTime.Now <= challenge.EndDate ||
                                                         DateTime.Now == challenge.StartDate && DateTime.Now.Hour < challenge.EndDate.Hour))
                    {
                        var endingSoonNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} is ending soon!",
                            Link = "/WeeklyChallenges/Index",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        _context.Notifications.Add(endingSoonNotification);
                        
                    }

                    // Notificare la sfârșitul provocării
                    var endNotificationExists = _context.Notifications.Any(n =>
                        n.UserId == user.Id &&
                        n.Link == "/WeeklyChallenges/Index" &&
                        n.Message == $"Challenge {challenge.Title} has ended!");

                    if (!endNotificationExists && DateTime.Now > challenge.EndDate.AddDays(1))
                    {
                        var endNotification = new Notification
                        {
                            UserId = user.Id,
                            Message = $"Challenge {challenge.Title} has ended!",
                            Link = "/WeeklyChallenges/Index",
                            CreatedAt = DateTime.Now,
                            IsRead = false
                        };

                        

                        _context.Notifications.Add(endNotification);
                    }

                    _context.SaveChanges();
                }


            }


        }

        
    }
}
