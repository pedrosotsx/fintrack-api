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
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var despesas = await _context.Despesas
            .Where(d => d.UsuarioId == usuarioId)
            .OrderByDescending(d => d.Data)
            .Select(d => new
            {
                d.Id,
                d.Descricao,
                d.Valor,
                d.Data,
                d.CategoriaId,
                Categoria = new
                {
                    d.Categoria.Id,
                    d.Categoria.Nome
                }
            })
            .ToListAsync();

        return Ok(despesas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var despesa = await _context.Despesas
            .Where(d =>
                d.Id == id &&
                d.UsuarioId == usuarioId)
            .Select(d => new
            {
                d.Id,
                d.Descricao,
                d.Valor,
                d.Data,
                d.CategoriaId,
                Categoria = new
                {
                    d.Categoria.Id,
                    d.Categoria.Nome
                }
            })
            .FirstOrDefaultAsync();

        if (despesa == null)
            return NotFound("Despesa não encontrada.");

        return Ok(despesa);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateDespesaDto dto)
    {
        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == dto.CategoriaId);

        if (!categoriaExiste)
            return BadRequest("Categoria inválida.");

        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var despesa = new Despesa
        {
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Data = DateTime.SpecifyKind(dto.Data, DateTimeKind.Utc),
            CategoriaId = dto.CategoriaId,
            UsuarioId = usuarioId
        };

        _context.Despesas.Add(despesa);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = despesa.Id },
            new
            {
                despesa.Id,
                despesa.Descricao,
                despesa.Valor,
                despesa.Data,
                despesa.CategoriaId
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CreateDespesaDto dto)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var despesaExistente = await _context.Despesas
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.UsuarioId == usuarioId);

        if (despesaExistente == null)
            return NotFound("Despesa não encontrada.");

        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == dto.CategoriaId);

        if (!categoriaExiste)
            return BadRequest("Categoria inválida.");

        despesaExistente.Descricao = dto.Descricao;
        despesaExistente.Valor = dto.Valor;
        despesaExistente.Data = DateTime.SpecifyKind(
            dto.Data,
            DateTimeKind.Utc);
        despesaExistente.CategoriaId = dto.CategoriaId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var despesa = await _context.Despesas
            .FirstOrDefaultAsync(d =>
                d.Id == id &&
                d.UsuarioId == usuarioId);

        if (despesa == null)
            return NotFound("Despesa não encontrada.");

        _context.Despesas.Remove(despesa);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
