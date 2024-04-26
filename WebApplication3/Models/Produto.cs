using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using WebApplication3.Areas.Identity.Data;

namespace WebApplication3.Models
{
    public class Produto
    {

        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(maximumLength: 255, MinimumLength = 8)]
        public string Nome { get; set; } = null!;

        [AllowNull]
        [StringLength(maximumLength: 2000)]
        public string Descricao { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(6,2)")]
        public decimal Preco { get; set; }

        
        public Usuario? Fornecedor { get; set; } = null!;


    }
}
