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
    .AsEnumerable()  // Mutam evaluarea pe client (memoria aplicatiei)
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

            // Definim pipeline-ul
            var pipeline = mlContext.Transforms.Concatenate("Features", "Features")
                .Append(mlContext.Clustering.Trainers.KMeans("Features", numberOfClusters: 3));
            foreach (var user in userEmotionVectors)
            {
                Console.WriteLine($"UserId: {user.UserId}, Features: {string.Join(",", user.Features)}");
                Console.WriteLine($"Vector Length: {user.Features.Length}", " type: ", user.Features.GetType());
                Console.WriteLine($"userEmotionVectors.GetType(): {userEmotionVectors.GetType()}");
            }
            // Incarcam si antrenam modelul
            var model = pipeline.Fit(data);

            // Transformam datele pentru a obtine predictiile
            var predictions = model.Transform(data);
            var clusterPredictions = mlContext.Data.CreateEnumerable<EmotionClusterPrediction>(predictions, reuseRowObject: false);

            // Actualizam utilizatorii cu clusterul atribuit
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
            // Obtinem emotiile existente in baza de date, care sunt fixate 7 la numar
            var allEmotions = _dbContext.Emotions.ToList();
            // Obtinem ID-urilor de jurnalele utilizatorului
            var diaryIds = _dbContext.Diaries
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .ToList();

            // Crearea unui dictionar cu scoruri pentru fiecare emotie
            var emotionScores = _dbContext.DiaryEmotions
                .Where(de => diaryIds.Contains(de.DiaryId.Value))
                .AsEnumerable()  
                .GroupBy(de => de.EmotionId)
                .ToDictionary(
                    g => g.Key,  
                    g => g.Average(de => de.Score) ?? 0.0);  // Media scorurilor pentru fiecare emotie

            // Crearea vectorului de scoruri pentru fiecare emotie 
            var emotionVector = new float[7];  
            // Alocarea de scoruri pentru fiecare emotie
            for (int i = 0; i < allEmotions.Count; i++)
            {
                var emotionId = allEmotions[i].Id;
                emotionVector[i] = emotionScores.ContainsKey(emotionId) ? (float)emotionScores[emotionId] : 0f;  
            }

            return emotionVector;
        }



    }
}
