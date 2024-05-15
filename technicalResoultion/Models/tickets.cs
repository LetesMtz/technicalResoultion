using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class tickets
    {
        [Key]
        public int id_ticket { get; set; }
        public int? id_categoria { get; set; }
        public string? nombre_problema { get; set; }
        public string? descripcion { get; set; }
        public string? archivos { get; set; }
        public DateTime? fecha_creacion { get; set; }
        public DateTime? fecha_ult_mod { get; set; }
        public int? id_estado_prioridad { get; set; }
        public int? id_estado_progreso { get; set; }
        public int? id_cliente { get; set; }
        public string? tipo_cliente { get; set; }
    }
}
