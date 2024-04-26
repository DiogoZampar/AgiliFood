using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class FechamentoMensalGeralDataModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        public string UsuarioId { get; set; }

        public string UsuarioNome { get; set; }
        
        public decimal ValorTotal { get; set; }

        public int NumPedidos { get; set; }


    }
}
