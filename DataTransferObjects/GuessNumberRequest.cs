using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.DataTransferObjects
{
    public class GuessNumberRequest
    {
        [Required(ErrorMessage = "El ID del juego es requerido")]
        public int GameId { get; set; }
        [Required(ErrorMessage = "Intente adivinar el numero")]
        public string AttemptedNumber { get; set; }
    }
}
