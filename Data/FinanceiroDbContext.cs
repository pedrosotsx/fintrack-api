using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Models;

namespace ControleFinanceiroApi.Data;

public class FinanceiroDbContext : DbContext
{
    public FinanceiroDbContext(DbContextOptions<FinanceiroDbContext> options)
        : base(options)
    {
    }

    public DbSet<Despesa> Despesas { get; set; }
}