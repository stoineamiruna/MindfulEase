using Microsoft.ML.Data;

namespace MindfulEase.Models
{
    public class UserData
    {
        
        public string UserId { get; set; }
        [VectorType(7)] // Definește coloana ca vector
        public float[] Features { get; set; }
    }

}
