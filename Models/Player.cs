using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public DateTime RegisterDate { get; set; }

    }
}
