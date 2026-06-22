namespace ControleFinanceiro.Models;

public class MetaFinanceira
{
    public int Id { get; set; }
    public string Nome { get; set; }

    public decimal ValorMeta { get; set; }
    public decimal ValorAtual { get; set; }

    public DateTime DataCriacao { get; set; }
}