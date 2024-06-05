using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class internos
    {
        [Key]
        public int id_interno {  get; set; }
        [Required(ErrorMessage = "Por favor, ingrese un nombre.")]
        public string? nombres_i {  get; set; }
        [Required(ErrorMessage = "Por favor, ingrese un apellido.")]
        public string? apellidos_i { get; set; }
        public string? tipo_usuario { get; set; }
        [EmailAddress(ErrorMessage = "Por favor, ingrese un correo electrónico válido.")]
        public string? correo_i { get; set; }
        [Required(ErrorMessage = "Por favor, ingrese una contraseña.")]
        public string? contrasena_i { get; set; }
        [Required(ErrorMessage = "Por favor, seleccione un rol.")]
        public int? id_role { get; set; }
    }
}
