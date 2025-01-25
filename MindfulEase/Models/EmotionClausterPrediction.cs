using Microsoft.ML.Data;

namespace MindfulEase.Models
{
    public class EmotionClusterPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint ClusterId { get; set; } // Cluster-ul prezis pentru utilizator

        [ColumnName("Score")]
        public float[] Distances { get; set; } // Distanțele până la fiecare cluster (opțional)
    }
}