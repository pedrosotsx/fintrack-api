using ControleFinanceiro.Models;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitasController : ControllerBase
{
    private readonly FinanceiroDbContext _context;

    public ReceitasController(FinanceiroDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var receitas = await _context.Receitas
            .Include(r => r.Categoria)
            .OrderByDescending(r => r.Data)
            .ToListAsync();

        return Ok(receitas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var receita = await _context.Receitas
            .Include(r => r.Categoria)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (receita == null)
            return NotFound("Receita não encontrada.");

        return Ok(receita);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Receita receita)
    {
        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == receita.CategoriaId);

        if (!categoriaExiste)
            return BadRequest("Categoria inválida.");

        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == receita.UsuarioId);

        if (!usuarioExiste)
            return BadRequest("Usuário inválido.");

        receita.Data = DateTime.SpecifyKind(
            receita.Data,
            DateTimeKind.Utc);

        _context.Receitas.Add(receita);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = receita.Id },
            receita);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Receita receita)
    {
        var receitaExistente = await _context.Receitas.FindAsync(id);

        if (receitaExistente == null)
            return NotFound("Receita não encontrada.");

        receitaExistente.Descricao = receita.Descricao;
        receitaExistente.Valor = receita.Valor;
        receitaExistente.Data = DateTime.SpecifyKind(
            receita.Data,
            DateTimeKind.Utc);

        receitaExistente.CategoriaId = receita.CategoriaId;
        receitaExistente.UsuarioId = receita.UsuarioId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var receita = await _context.Receitas.FindAsync(id);

        if (receita == null)
            return NotFound("Receita não encontrada.");

        _context.Receitas.Remove(receita);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}