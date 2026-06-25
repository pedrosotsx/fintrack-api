namespace ControleFinanceiro.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public ICollection<Receita> Receitas { get; set; } = new List<Receita>();

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();

    public ICollection<MetaFinanceira> MetasFinanceiras { get; set; } = new List<MetaFinanceira>();
}