using Microsoft.AspNetCore.Mvc;

namespace ControleFinanceiro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    [HttpGet]
    public IActionResult Listar()
    {
        return Ok("API funcionando");
    }
}