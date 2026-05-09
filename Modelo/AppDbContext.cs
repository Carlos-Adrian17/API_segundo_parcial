using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Paciente> Pacientes_Carlos_Marroquin_3364 { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Paciente>()
            .ToTable("pacientes_carlos_marroquin_3364");

        modelBuilder.Entity<Paciente>()
            .HasKey(p => p.PacienteId);

        base.OnModelCreating(modelBuilder);
    }
}