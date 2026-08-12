using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Models;

namespace MiniDocs.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<AuditoriaDocumento> AuditoriasDocumentos => Set<AuditoriaDocumento>();

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

        builder.Entity<Documento>()
            .HasOne(d => d.Departamento)
            .WithMany()
            .HasForeignKey(d => d.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AuditoriaDocumento>()
            .HasOne(a => a.Documento)
            .WithMany()
            .HasForeignKey(a => a.DocumentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AuditoriaDocumento>()
            .HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Documento>()
            .HasOne(d => d.Usuario)
            .WithMany()
            .HasForeignKey(d => d.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
