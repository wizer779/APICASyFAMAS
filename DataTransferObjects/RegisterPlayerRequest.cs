using System.ComponentModel.DataAnnotations;

namespace APICASyFAMAS.DataTransferObjects
{
    public class RegisterPlayerRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(2, 100, ErrorMessage = "La edad debe estar entre 2 y 100.")]
        public int Age { get; set; }
    }
}
