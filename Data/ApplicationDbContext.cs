using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Models;

namespace MiniDocs.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Departamento> Departamentos => Set<Departamento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Departamento>()
            .HasIndex(d => d.Nombre)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Departamento)
            .WithMany()
            .HasForeignKey(u => u.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
