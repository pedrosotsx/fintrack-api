namespace ControleFinanceiro.DTOs;

public class CreateReceitaDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public int CategoriaId { get; set; }
}