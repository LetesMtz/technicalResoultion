using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class roles
    {
        [Key]
        public int id_role { get; set; }
        public string? nombre_role { get; set; }
        public string? permisos_descripcion { get; set; }
    }
}
