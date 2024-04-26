using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Models;

namespace WebApplication3.ViewModels
{
    public class PedidoDataModel
    {
        public int ProdutoId { get; set; }

        public string ProdutoNome { get; set; }

        public decimal ProdutoPreco { get; set; }

        public string ProdutoFornecedorId { get; set; }

        public int Quantidade { get; set; }

    }
}
