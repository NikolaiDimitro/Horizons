using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizons.Data.Entities
{
    public class Destination
    {
        public Destination()
        {
            IsDeleted = false;
            UsersDestinations = new HashSet<UserDestination>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; } = null!;

        public string? ImageUrl { get; set; }

        [Required]
        [ForeignKey(nameof(Publisher))]
        public string PublisherId { get; set; }

        [Required]
        public IdentityUser Publisher { get; set; }

        [Required]
        public DateTime PublishedOn { get; set; }

        [Required]
        [ForeignKey(nameof(Terrain))]
        public int TerrainId { get; set; }

        [Required]
        public Terrain Terrain { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<UserDestination> UsersDestinations { get; set; }
    }
}

