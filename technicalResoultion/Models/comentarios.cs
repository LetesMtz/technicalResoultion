using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class comentarios
    {
        [Key]
        public int id_comentario { get; set; }
        public string? comentario { get; set; }
        public DateTime? fecha { get; set; }
        public int? id_ticket { get; set; }
        public int? id_interno { get; set; }
    }
}
