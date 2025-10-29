using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.Models
{
    public class Attempt
    {
        [Key]
        public int AttemptId { get; set; }
        [Required]
        public int GameId { get; set; }
        [Required]
        public string AttemptedNumber { get; set; }
        [Required]
        public int Famas { get; set; }
        [Required]
        public int Picas { get; set; }
        [Required]
        public DateTime AttemptDate { get; set; }
        public string Message { get; set; }
    }
}
