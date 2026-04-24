using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Horizons.Data.Entities
{
    public class Terrain
    {
        public Terrain()
        {
            Destinations = new HashSet<Destination>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        public virtual IEnumerable<Destination> Destinations { get; set; }

    }
}


