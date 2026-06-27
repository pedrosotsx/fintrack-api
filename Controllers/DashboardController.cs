using System.Security.Claims;
using ControleFinanceiro.DTOs;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly FinanceiroDbContext _context;

    public DashboardController(FinanceiroDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usuarioId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var totalReceitas = await _context.Receitas
            .Where(r => r.UsuarioId == usuarioId)
            .SumAsync(r => (decimal?)r.Valor) ?? 0;

        var totalDespesas = await _context.Despesas
            .Where(d => d.UsuarioId == usuarioId)
            .SumAsync(d => (decimal?)d.Valor) ?? 0;

        var quantidadeReceitas = await _context.Receitas
            .Where(r => r.UsuarioId == usuarioId)
            .CountAsync();

        var quantidadeDespesas = await _context.Despesas
            .Where(d => d.UsuarioId == usuarioId)
            .CountAsync();

        var quantidadeMetas = await _context.MetasFinanceiras
            .Where(m => m.UsuarioId == usuarioId)
            .CountAsync();

        var dashboard = new DashboardDto
        {
            TotalReceitas = totalReceitas,
            TotalDespesas = totalDespesas,
            Saldo = totalReceitas - totalDespesas,
            QuantidadeReceitas = quantidadeReceitas,
            QuantidadeDespesas = quantidadeDespesas,
            QuantidadeMetas = quantidadeMetas
        };

        return Ok(dashboard);
    }
}