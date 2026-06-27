using System.Security.Claims;
using ControleFinanceiro.DTOs;
using ControleFinanceiro.Models;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;

[Authorize]
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
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var metas = await _context.MetasFinanceiras
            .Where(m => m.UsuarioId == usuarioId)
            .OrderByDescending(m => m.DataCriacao)
            .Select(m => new
            {
                m.Id,
                m.Nome,
                m.ValorMeta,
                m.ValorAtual,
                m.DataCriacao
            })
            .ToListAsync();

        return Ok(metas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var meta = await _context.MetasFinanceiras
            .Where(m =>
                m.Id == id &&
                m.UsuarioId == usuarioId)
            .Select(m => new
            {
                m.Id,
                m.Nome,
                m.ValorMeta,
                m.ValorAtual,
                m.DataCriacao
            })
            .FirstOrDefaultAsync();

        if (meta == null)
            return NotFound("Meta não encontrada.");

        return Ok(meta);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateMetaDto dto)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var meta = new MetaFinanceira
        {
            Nome = dto.Nome,
            ValorMeta = dto.ValorMeta,
            ValorAtual = dto.ValorAtual,
            UsuarioId = usuarioId,
            DataCriacao = DateTime.UtcNow
        };

        _context.MetasFinanceiras.Add(meta);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = meta.Id },
            new
            {
                meta.Id,
                meta.Nome,
                meta.ValorMeta,
                meta.ValorAtual,
                meta.DataCriacao
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CreateMetaDto dto)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var metaExistente = await _context.MetasFinanceiras
            .FirstOrDefaultAsync(m =>
                m.Id == id &&
                m.UsuarioId == usuarioId);

        if (metaExistente == null)
            return NotFound("Meta não encontrada.");

        metaExistente.Nome = dto.Nome;
        metaExistente.ValorMeta = dto.ValorMeta;
        metaExistente.ValorAtual = dto.ValorAtual;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var meta = await _context.MetasFinanceiras
            .FirstOrDefaultAsync(m =>
                m.Id == id &&
                m.UsuarioId == usuarioId);

        if (meta == null)
            return NotFound("Meta não encontrada.");

        _context.MetasFinanceiras.Remove(meta);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
