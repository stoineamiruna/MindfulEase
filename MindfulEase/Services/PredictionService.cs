namespace MindfulEase.Services
{
    public class PredictionService
    {
        public object PredictBrainState(string userId, int years)
        {
            // Logică simplă de test: creșterea emoțiilor negative în timp
            return new
            {
                Region = "Amigdala",
                Color = "#ff0000",
                Message = $"Predicție peste {years} ani: anxietate crescută."
            };
        }
    }
}
