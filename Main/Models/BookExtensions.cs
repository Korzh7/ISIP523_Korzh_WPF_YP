using System.Linq;

namespace Main.Models
{
    public partial class Book
    {
        public double AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        public string AverageRatingText => Reviews.Any() ? $"★ {AverageRating:F1}" : "★ —";
    }
}