using System.ComponentModel.DataAnnotations;

namespace VisaNet.VONRegister.Models
{
    public class SignIn
    {
        [Required(ErrorMessage = "El email es requerido")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public bool EditableEmail { get; set; }
    }
}