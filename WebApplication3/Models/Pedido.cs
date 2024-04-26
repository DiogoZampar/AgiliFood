using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebApplication3.Areas.Identity.Data;

namespace WebApplication3.Models
{
    public class Pedido
    {

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public decimal ValorTotal { get; set; }

        public DateTime? DataPedido { get; set; }

        public Usuario? Cliente { get; set; } = null!;

        public Usuario? Fornecedor { get; set; } = null!;

        public List<DetalhesPedido> DetalhesPedidos { get; set; } = null!;


    }
}
