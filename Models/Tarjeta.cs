using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Api.Models
{
    [Table("Tarjeta")]
    public class Tarjeta
    {
        public int Id { get; set; }
        public string NombreTitular { get; set; }
        public string NumeroTarjeta { get; set; }
        public string Estado { get; set; }
        public bool Activa { get; set; }
    }
}
