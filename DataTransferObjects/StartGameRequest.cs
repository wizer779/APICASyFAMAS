using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.DataTransferObjects
{
    public class StartGameRequest
    {
        [Required(ErrorMessage = "El ID del jugador es requerido")]
        public int PlayerId { get; set; }
    }
}
