using ControleFinanceiro.DTOs;
using ControleFinanceiro.Models;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ControleFinanceiro.Controllers;

[Authorize]
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
        var usuarioId = GetUserId();

        var receitas = await _context.Receitas
            .Where(r => r.UsuarioId == usuarioId)
            .OrderByDescending(r => r.Data)
            .Select(r => new
            {
                r.Id,
                r.Descricao,
                r.Valor,
                r.Data,
                r.CategoriaId,
                Categoria = new
                {
                    r.Categoria.Id,
                    r.Categoria.Nome
                }
            })
            .ToListAsync();

        return Ok(receitas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuarioId = GetUserId();

        var receita = await _context.Receitas
            .Where(r =>
                r.Id == id &&
                r.UsuarioId == usuarioId)
            .Select(r => new
            {
                r.Id,
                r.Descricao,
                r.Valor,
                r.Data,
                r.CategoriaId,
                Categoria = new
                {
                    r.Categoria.Id,
                    r.Categoria.Nome
                }
            })
            .FirstOrDefaultAsync();

        if (receita == null)
            return NotFound("Receita não encontrada.");

        return Ok(receita);
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateReceitaDto dto)
    {
        var usuarioId = GetUserId();

        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == dto.CategoriaId);

        if (!categoriaExiste)
            return BadRequest("Categoria inválida.");

        var receita = new Receita
        {
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Data = DateTime.SpecifyKind(dto.Data, DateTimeKind.Utc),
            CategoriaId = dto.CategoriaId,
            UsuarioId = usuarioId
        };

        _context.Receitas.Add(receita);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = receita.Id },
            new
            {
                receita.Id,
                receita.Descricao,
                receita.Valor,
                receita.Data,
                receita.CategoriaId
            });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdateReceitaDto dto)
    {
        var usuarioId = GetUserId();

        var receitaExistente = await _context.Receitas
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.UsuarioId == usuarioId);

        if (receitaExistente == null)
            return NotFound("Receita não encontrada.");

        var categoriaExiste = await _context.Categorias
            .AnyAsync(c => c.Id == dto.CategoriaId);

        if (!categoriaExiste)
            return BadRequest("Categoria inválida.");

        receitaExistente.Descricao = dto.Descricao;
        receitaExistente.Valor = dto.Valor;
        receitaExistente.Data = DateTime.SpecifyKind(dto.Data, DateTimeKind.Utc);
        receitaExistente.CategoriaId = dto.CategoriaId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuarioId = GetUserId();

        var receita = await _context.Receitas
            .FirstOrDefaultAsync(r =>
                r.Id == id &&
                r.UsuarioId == usuarioId);

        if (receita == null)
            return NotFound("Receita não encontrada.");

        _context.Receitas.Remove(receita);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private int GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId) || !int.TryParse(userId, out var id))
            throw new InvalidOperationException("Usuário não autenticado ou token inválido.");

        return id;
    }
}
