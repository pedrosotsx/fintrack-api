using ControleFinanceiro.Models;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetasController : ControllerBase
{
    private readonly FinanceiroDbContext _context;

    public MetasController(FinanceiroDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var metas = await _context.MetasFinanceiras
            .OrderByDescending(m => m.DataCriacao)
            .ToListAsync();

        return Ok(metas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var meta = await _context.MetasFinanceiras
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meta == null)
            return NotFound("Meta não encontrada.");

        return Ok(meta);
    }

    [HttpPost]
    public async Task<IActionResult> Post(MetaFinanceira meta)
    {
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.Id == meta.UsuarioId);

        if (!usuarioExiste)
            return BadRequest("Usuário inválido.");

        meta.DataCriacao = DateTime.UtcNow;

        _context.MetasFinanceiras.Add(meta);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = meta.Id },
            meta);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, MetaFinanceira meta)
    {
        var metaExistente = await _context.MetasFinanceiras
            .FindAsync(id);

        if (metaExistente == null)
            return NotFound("Meta não encontrada.");

        metaExistente.Nome = meta.Nome;
        metaExistente.ValorMeta = meta.ValorMeta;
        metaExistente.ValorAtual = meta.ValorAtual;
        metaExistente.UsuarioId = meta.UsuarioId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var meta = await _context.MetasFinanceiras
            .FindAsync(id);

        if (meta == null)
            return NotFound("Meta não encontrada.");

        _context.MetasFinanceiras.Remove(meta);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}