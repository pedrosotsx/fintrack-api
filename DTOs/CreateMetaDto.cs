namespace ControleFinanceiro.DTOs;

public class CreateMetaDto
{
    public string Nome { get; set; } = string.Empty;

    public decimal ValorMeta { get; set; }

    public decimal ValorAtual { get; set; }
}
