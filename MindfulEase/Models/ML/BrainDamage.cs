using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Models.ML
{
    public class BrainDamage
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Vârsta trebuie să fie între 1 și 100")]
        public float Age { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Joy { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Sadness { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Anger { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Love { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Fear { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Surprise { get; set; }

        [Required]
        [Range(0, 10, ErrorMessage = "Valoarea trebuie să fie între 0 și 10")]
        public float Disgust { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Anii de predicție trebuie să fie între 1 și 10")]
        public int YearsSinceStart { get; set; } = 5;

        public Dictionary<string, float> PredictedDamage { get; set; }

        public List<EmotionData> Emotions { get; set; }

        public List<BrainRegionData> BrainRegions { get; set; }

        public List<EmotionBrainRegionData> EmotionBrainRegions { get; set; }

        public EmotionalInputData ToEmotionalInputData()
        {
            return new EmotionalInputData
            {
                Age = Age,
                Sex = Sex,
                Joy = Joy,
                Sadness = Sadness,
                Anger = Anger,
                Love = Love,
                Fear = Fear,
                Surprise = Surprise,
                Disgust = Disgust,
                YearsSinceStart = YearsSinceStart
            };
        }
    }

    public class EmotionData
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string ColorCode { get; set; }
    }

    public class BrainRegionData
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class EmotionBrainRegionData
    {
        public int EmotionId { get; set; }
        public int BrainRegionId { get; set; }
    }
}
