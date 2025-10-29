using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public string SecretNumber { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        [Required]
        public bool IsFinished { get; set; }


    }
}
