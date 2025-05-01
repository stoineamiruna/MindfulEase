using Microsoft.ML.Data;

namespace MindfulEase.Models.ML
{
    public class EmotionalBrainData
    {
        [LoadColumn(0)]
        public float ParticipantID { get; set; }

        [LoadColumn(1)]
        public float Age { get; set; }
        [LoadColumn(2)]
        public string Sex { get; set; }

        [LoadColumn(3)]
        
        public float YearsSinceStart { get; set; }

        // Emotions
        [LoadColumn(4)]
        public float Joy { get; set; }

        [LoadColumn(5)]
        public float Sadness { get; set; }

        [LoadColumn(6)]
        public float Anger { get; set; }

        [LoadColumn(7)]
        public float Love { get; set; }

        [LoadColumn(8)]
        public float Fear { get; set; }

        [LoadColumn(9)]
        public float Surprise { get; set; }

        [LoadColumn(10)]
        public float Disgust { get; set; }

        // Brain Regions
        [LoadColumn(11)]
        public float VentromedialPrefrontalCortex { get; set; }

        [LoadColumn(12)]
        public float NucleusAccumbens { get; set; }

        [LoadColumn(13)]
        public float Amygdala { get; set; }

        [LoadColumn(14)]
        public float AnteriorCingulateCortex { get; set; }

        [LoadColumn(15)]
        public float Insula { get; set; }

        [LoadColumn(16)]
        public float Hypothalamus { get; set; }

        [LoadColumn(17)]
        public float DorsolateralPrefrontalCortex { get; set; }

        [LoadColumn(18)]
        public float OrbitofrontalCortex { get; set; }

        [LoadColumn(19)]
        public float Striatum { get; set; }

        [LoadColumn(20)]
        public float Hippocampus { get; set; }

        [LoadColumn(21)]
        public float SuperiorParietalCortex { get; set; }

        [LoadColumn(22)]
        public float BasalGanglia { get; set; }
    }
}
