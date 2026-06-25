namespace ControleFinanceiro.Models;

public class MetaFinanceira
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public decimal ValorMeta { get; set; }

    public decimal ValorAtual { get; set; }

    public DateTime DataCriacao { get; set; }

    public int UsuarioId { get; set; }

    public Usuario Usuario { get; set; } = null!;
}