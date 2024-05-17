using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class asignar_tareas
    {
        [Key]
        public int id_tarea { get; set; }
        public int? id_ticket { get; set; }
        public string? tarea { get; set; }
        public int? id_estado_progreso { get; set; }
        public int? id_interno { get; set; }
    }
}
