using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Horizons.Data.Entities
{
    public class UserDestination
    {
        // User
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        public IdentityUser User { get; set; }

        // Destination
        [Required]
        [ForeignKey(nameof(Destination))]
        public int DestinationId { get; set; }

        [Required]
        public Destination Destination { get; set; }


    }
}