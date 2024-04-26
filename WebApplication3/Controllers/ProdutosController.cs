using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Areas.Identity.Data;
using WebApplication3.Models;


namespace WebApplication3.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly AgiliFoodDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;



        public ProdutosController(AgiliFoodDbContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Produtos
        public async Task<IActionResult> Index(string id)
        {
            return View(await _context.Produtos.Include(a => a.Fornecedor).Where(a => a.Fornecedor.Id == id).ToListAsync());
        }

        // GET: Produtos/FornecedorIndex
        public async Task<IActionResult> FornecedorIndex()
        {

            var ListaFornecedores = await _userManager.GetUsersInRoleAsync("FornecedorAtivo");

            return View(ListaFornecedores);
        }

        // GET: Produtos/Fornecedor/5
        public async Task<IActionResult> Fornecedor(int id)
        {

            var ListaProdutos = await _context.Produtos.Where(a => a.Fornecedor.Id == id.ToString()).ToListAsync();

            return View(ListaProdutos);
        }




        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.Include(a => a.Fornecedor)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produtos/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,Preco")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                produto.Fornecedor = _userManager.GetUserAsync(currentUser).Result;
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = produto.Fornecedor.Id.ToString() });
            }
            return View(produto);
        }

        // GET: Produtos/Edit/5
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var produto = await _context.Produtos.
                            Include(produto => produto.Fornecedor).
                            FirstOrDefaultAsync(m => m.ID == id);


            if (produto == null)
            {
                return NotFound();
            }

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            if (produto.Fornecedor.Id.Equals(_userManager.GetUserAsync(currentUser).Result.Id))
            {
                return View(produto);
            }

            return Unauthorized();
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> Edit([Bind("ID,Nome,Descricao,Preco")] Produto produto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                System.Security.Claims.ClaimsPrincipal currentUser = this.User;
                return RedirectToAction(nameof(Index), new { id = _userManager.GetUserAsync(currentUser).Result.Id });
            }
            return View(produto);
        }

        // GET: Produtos/Delete/5
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();

            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            return RedirectToAction(nameof(Index), new { id = _userManager.GetUserAsync(currentUser).Result.Id });
        }

        // GET: Produtos/DesativarProdutos
        [HttpGet]
        [Authorize(Roles = ("Fornecedor"))]
        [Authorize(Roles = ("FornecedorAtivo"))]
        public async Task<IActionResult> DesativarProdutos()
        {
            var user = await _userManager.GetUserAsync(this.User);
            await _userManager.RemoveFromRoleAsync(user, "FornecedorAtivo");
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction(nameof(Index), new { id = user.Id });
        }

        // GET: Produtos/AtivarProdutos
        [HttpGet]
        [Authorize(Roles = ("Fornecedor"))]
        public async Task<IActionResult> AtivarProdutos()
        {
            var user = await _userManager.GetUserAsync(this.User);
            await _userManager.AddToRoleAsync(user, "FornecedorAtivo");
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToAction(nameof(Index), new { id = user.Id });
        }


        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.ID == id);
        }
    }
}
