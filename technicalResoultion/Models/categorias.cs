using System.ComponentModel.DataAnnotations;

namespace technicalResoultion.Models
{
    public class categorias
    {
        [Key]
        public int id_categoria { get; set; }
        public string? categoria { get; set; }
    }
}
