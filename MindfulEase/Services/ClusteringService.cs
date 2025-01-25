using Microsoft.AspNetCore.Mvc;
using global::MindfulEase.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MindfulEase.Models;
using System.Linq;
using static Humanizer.In;

namespace MindfulEase.Services
{
    public class ClusteringService
    {
        private readonly ApplicationDbContext _dbContext;

        public ClusteringService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AssignClustersToUsers()
        {
            var userEmotionVectors = _dbContext.ApplicationUsers
    .AsEnumerable()  // Mută evaluarea pe client (memoria aplicației)
    .Select(user => new UserData
    {
        UserId = user.Id,
        Features = GetEmotionVector(user.Id)
    })
    .Where(u => u.Features.Length > 0)
    .ToList();


            if (!userEmotionVectors.Any()) return;

            var mlContext = new MLContext();
            var data = mlContext.Data.LoadFromEnumerable(userEmotionVectors);

            // Definește pipeline-ul
            var pipeline = mlContext.Transforms.Concatenate("Features", "Features")
                .Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));
            foreach (var user in userEmotionVectors)
            {
                Console.WriteLine($"UserId: {user.UserId}, Features: {string.Join(",", user.Features)}");
                Console.WriteLine($"Vector Length: {user.Features.Length}", " type: ", user.Features.GetType());
                Console.WriteLine($"userEmotionVectors.GetType(): {userEmotionVectors.GetType()}");
            }
            // Încarcă și antrenează modelul
            var model = pipeline.Fit(data);

            // Transformă datele pentru a obține predicțiile
            var predictions = model.Transform(data);
            var clusterPredictions = mlContext.Data.CreateEnumerable<EmotionClusterPrediction>(predictions, reuseRowObject: false);

            // Actualizează utilizatorii cu clusterul atribuit
            foreach (var (user, prediction) in userEmotionVectors.Zip(clusterPredictions))
            {
                var userEntity = _dbContext.ApplicationUsers.Find(user.UserId);
                if (userEntity != null)
                {
                    userEntity.ClusterId = (int)prediction.ClusterId;
                }
            }

            _dbContext.SaveChanges();

        }

        private float[] GetEmotionVector(string userId)
        {
            // Obține toate emoțiile existente (din baza de date), care sunt acum fixate la 7
            var allEmotions = _dbContext.Emotions.ToList();

            // Obține toate ID-urile de jurnale pentru utilizator
            var diaryIds = _dbContext.Diaries
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .ToList();

            // Creează un dicționar cu scoruri pentru fiecare emoție
            var emotionScores = _dbContext.DiaryEmotions
                .Where(de => diaryIds.Contains(de.DiaryId.Value))
                .AsEnumerable()  // Forțează evaluarea pe client
                .GroupBy(de => de.EmotionId)
                .ToDictionary(
                    g => g.Key,  // EmotionId
                    g => g.Average(de => de.Score) ?? 0.0);  // Media scorurilor pentru fiecare emoție

            // Creează vectorul de scoruri pentru fiecare emoție din toate emoțiile posibile
            var emotionVector = new float[7];  // Dimensiunea fixă

            // Umple vectorul cu scoruri pentru fiecare emoție
            for (int i = 0; i < allEmotions.Count; i++)
            {
                var emotionId = allEmotions[i].Id;
                emotionVector[i] = emotionScores.ContainsKey(emotionId) ? (float)emotionScores[emotionId] : 0f;  // Folosește scorul sau 0 dacă nu există scor
            }

            return emotionVector;
        }



    }
}
