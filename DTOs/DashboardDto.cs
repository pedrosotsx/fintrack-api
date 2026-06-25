namespace ControleFinanceiro.DTOs;

public class DashboardDto
{
    public decimal TotalReceitas { get; set; }

    public decimal TotalDespesas { get; set; }

    public decimal Saldo { get; set; }

    public int QuantidadeReceitas { get; set; }

    public int QuantidadeDespesas { get; set; }

    public int QuantidadeMetas { get; set; }
}