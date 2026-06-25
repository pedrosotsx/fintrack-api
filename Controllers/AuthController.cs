using BCrypt.Net;
using ControleFinanceiro.DTOs;
using ControleFinanceiro.Models;
using ControleFinanceiro.Services;
using ControleFinanceiroApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiro.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FinanceiroDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(
        FinanceiroDbContext context,
        JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExiste)
            return BadRequest("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        _context.Usuarios.Add(usuario);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            mensagem = "Usuário cadastrado com sucesso."
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null)
            return Unauthorized("E-mail ou senha inválidos.");

        bool senhaValida = BCrypt.Net.BCrypt.Verify(
            dto.Senha,
            usuario.SenhaHash);

        if (!senhaValida)
            return Unauthorized("E-mail ou senha inválidos.");

        var token = _jwtService.GerarToken(usuario);

        return Ok(new
        {
            token
        });
    }
}