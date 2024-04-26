using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using WebApplication3.Areas.Identity.Data;
using WebApplication3.Migrations;
using WebApplication3.Models;
using WebApplication3.ViewModels;

namespace WebApplication3.Controllers
{
    public class PedidosController : Controller
    {
        private readonly AgiliFoodDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public PedidosController(AgiliFoodDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Pedidos/IndexCliente
        public async Task<IActionResult> IndexCliente([FromRoute]string id)
        {
            return View("Index", await _context.Pedidos.Where(a => a.Cliente.Id.ToString() == id).Include(a => a.Fornecedor).Include(a => a.Cliente).OrderByDescending(a => a.DataPedido).ToListAsync()) ;
        }


        
        // GET: Pedidos/IndexRestaurante
        public async Task<IActionResult> IndexFornecedor([FromRoute] string id)
        {



            return View("Index", await _context.Pedidos.Where(a => a.Fornecedor.Id.ToString() == id).Include(a => a.Fornecedor).Include(a => a.Cliente).OrderByDescending(a => a.DataPedido).ToListAsync());
        }



        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(a => a.Fornecedor)
                .Include(a => a.DetalhesPedidos)
                .Include(b => b.DetalhesPedidos).ThenInclude(b => b.Produto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        
        //GET: Pedidos/CardapioFornecedor/4878ecfa-2d84-4ec1-b0c8-07281a680bce
        public async Task<IActionResult> CardapioFornecedor(string? id)
        {

            if (id == null)
            {
                return NotFound();
            }



            var models = new List<PedidoDataModel>();
            var produtos = await _context.Produtos.Include(a => a.Fornecedor).OrderBy(c => c.Preco).Where(a => a.Fornecedor.Id == id).ToListAsync();
            if (produtos.Any())
            {
                for (int i = 0; i < produtos.Count; i++)
                {

                    var model = new PedidoDataModel();
                    model.ProdutoId = produtos[i].ID;
                    model.ProdutoPreco = produtos[i].Preco;
                    model.ProdutoNome = produtos[i].Nome;
                    model.ProdutoFornecedorId = produtos[i].Fornecedor.Id;
                    model.Quantidade = 0;
                    models.Add(model);
                }
            }
            return View(models);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Funcionario,Financeiro"))]
        public async Task<IActionResult> Create(IEnumerable<PedidoDataModel> models)
        {

            Pedido pedido = new Pedido();
            pedido.DetalhesPedidos = new List<DetalhesPedido>();
            decimal ValorTotal = 0;
            for (int i=0;i<models.Count();i++)
            {
                if (models.ElementAt(i).Quantidade > 0)
                { 
                    DetalhesPedido detalhes = new DetalhesPedido();
                    detalhes.Quantidade = models.ElementAt(i).Quantidade;                    
                    detalhes.Produto = await _context.Produtos.FindAsync(models.ElementAt(i).ProdutoId);
                    pedido.DetalhesPedidos.Add(detalhes);
                    ValorTotal += models.ElementAt(i).ProdutoPreco * models.ElementAt(i).Quantidade;
                }
            }
            pedido.ValorTotal = ValorTotal;

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            pedido.Cliente = _userManager.GetUserAsync(currentUser).Result;

            pedido.Fornecedor = await _userManager.FindByIdAsync(models.ElementAt(0).ProdutoFornecedorId);

            pedido.DataPedido = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(IndexCliente), new {id = pedido.Cliente.Id.ToString() });
            }

            return View("Views/Home/Index.cshtml");
        }


     
        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.ID == id);
        }

        //GET: Pedidos/IndexMensal
        [HttpGet]
        [Authorize(Roles = ("Financeiro"))]
        public async Task<IActionResult> IndexMensal(string id, int mes, int ano)
        {

            var pedidos = _context.Pedidos
                .Include(a => a.Cliente)
                .Include(a => a.Fornecedor)
                .Where(a => a.DataPedido.Value.Month == mes)
                .Where(a => a.DataPedido.Value.Year == ano)
                .Where(a => a.Cliente.Id == id)
                .OrderByDescending(a => a.DataPedido);

            
            return View(pedidos);
        }

        //GET: Pedidos/FechamentoMensal
        [HttpGet, ActionName("FechamentoMensal")]
        [Authorize(Roles = ("Financeiro"))]
        public async Task<IActionResult> FechamentoMensal(int mes, int ano)
        {
            ViewBag.ano = ano.ToString();
            ViewBag.mes = mes.ToString();


            var resultado = _context.Pedidos
            .Where(a => a.DataPedido.Value.Month == mes)
            .Where(a => a.DataPedido.Value.Year == ano)
            .GroupBy(a => a.Cliente.Id)
            .Select(a => new { Total = a.Sum(b => b.ValorTotal), UsuarioId = a.Key, NumPedidos = a.Count(c => c.Cliente.Id != null)})
            .OrderByDescending(a => a.Total)
            .ToList();




            var viewModelList = new List<FechamentoMensalGeralDataModel>();
            foreach(var model in resultado)
            {
                var vm = new FechamentoMensalGeralDataModel();
                vm.ValorTotal = model.Total;
                vm.NumPedidos = model.NumPedidos;
                vm.UsuarioId = model.UsuarioId.ToString();
                vm.UsuarioNome =  _context.Users.FirstOrDefault(u => u.Id == model.UsuarioId.ToString()).Nome;
                viewModelList.Add(vm);
            }


            return View(viewModelList);

        }

    }
}
