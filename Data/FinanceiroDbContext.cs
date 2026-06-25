using ControleFinanceiro.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleFinanceiroApi.Data;

public class FinanceiroDbContext : DbContext
{
    public FinanceiroDbContext(DbContextOptions<FinanceiroDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }

    public DbSet<Receita> Receitas { get; set; }

    public DbSet<Despesa> Despesas { get; set; }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Usuario>()
        .HasIndex(u => u.Email)
        .IsUnique();

    modelBuilder.Entity<Usuario>()
        .HasMany(u => u.Receitas)
        .WithOne(r => r.Usuario)
        .HasForeignKey(r => r.UsuarioId);

    modelBuilder.Entity<Usuario>()
        .HasMany(u => u.Despesas)
        .WithOne(d => d.Usuario)
        .HasForeignKey(d => d.UsuarioId);

    modelBuilder.Entity<Usuario>()
        .HasMany(u => u.MetasFinanceiras)
        .WithOne(m => m.Usuario)
        .HasForeignKey(m => m.UsuarioId);

    modelBuilder.Entity<Categoria>()
        .HasMany(c => c.Receitas)
        .WithOne(r => r.Categoria)
        .HasForeignKey(r => r.CategoriaId);

    modelBuilder.Entity<Categoria>()
        .HasMany(c => c.Despesas)
        .WithOne(d => d.Categoria)
        .HasForeignKey(d => d.CategoriaId);
}
}