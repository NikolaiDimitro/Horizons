using System.ComponentModel.DataAnnotations;

namespace Horizons.Models
{
    public class AddDestinationViewModel
    {
        public AddDestinationViewModel()
        {
            Terrains = new List<TerrainViewModel>();
        }

        [Required]
        [StringLength(80, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        public int TerrainId { get; set; }

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        public string PublisherId { get; set; } = null!;

        [Required]
        public string PublishedOn { get; set; }

        public IEnumerable<TerrainViewModel> Terrains { get; set; } = null!;
    }
}
