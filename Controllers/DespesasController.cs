using ControleFinanceiro.Models;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DespesaController : ControllerBase
{
    private readonly FinanceiroDbContext _context;

    public DespesaController(FinanceiroDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var despesa = await _context.Despesas
            .Include(r => r.Categoria)
            .ToListAsync();

        return Ok(despesa);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var despesa = await _context.Despesas
            .Include(r => r.Categoria)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (despesa == null)
            return NotFound();

        return Ok(despesa);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Despesa despesa)
    {
        despesa.Data = DateTime.SpecifyKind(
            despesa.Data,
            DateTimeKind.Utc);

        _context.Despesas.Add(despesa);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = despesa.Id },
            despesa);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Despesa despesa)
    {
        var DespesaExistente = await _context.Despesas.FindAsync(id);

        if (DespesaExistente == null)
            return NotFound();

        DespesaExistente.Descricao = despesa.Descricao;
        DespesaExistente.Valor = despesa.Valor;
        DespesaExistente.Data = DateTime.SpecifyKind(
            despesa.Data,
            DateTimeKind.Utc);

        DespesaExistente.CategoriaId = despesa.CategoriaId;
        DespesaExistente.UsuarioId = despesa.UsuarioId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var despesa = await _context.Despesas.FindAsync(id);

        if (despesa == null)
            return NotFound();

        _context.Despesas.Remove(despesa);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}