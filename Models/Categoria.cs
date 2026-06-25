namespace ControleFinanceiro.Models;
public class Categoria
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public ICollection<Receita> Receitas { get; set; } = new List<Receita>();

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}