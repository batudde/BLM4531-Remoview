using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace remoview.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Örn: "Action", "Sci-Fi"

        // Bir türün birden fazla filmi olabilir
        public virtual ICollection<Film> Films { get; set; } = new List<Film>();
    }
}