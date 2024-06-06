using System.ComponentModel.DataAnnotations;
namespace technicalResoultion.Models
{
    public class dashboard
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltMod { get; set; }
        public string Estado { get; set; }
    }
}
