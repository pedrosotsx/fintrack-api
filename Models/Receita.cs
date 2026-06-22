namespace ControleFinanceiro.Models;

public class Receita : Categoria
{
    public int Id { get; set; }
    public string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; }
}