namespace Horizons.Models
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Terrain { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Publisher { get; set; } = null!;
        public DateTime PublishedOn { get; set; } 
        public bool IsFavorite { get; set; }
        public bool IsPublisher { get; set; }
    }
}
