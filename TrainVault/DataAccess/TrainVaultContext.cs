using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TrainVault.DataAccess;

public partial class TrainVaultContext : DbContext
{
    public TrainVaultContext()
    {
    }

    public TrainVaultContext(DbContextOptions<TrainVaultContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Training> Trainings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("server=DESKTOP-LI40AFI\\SQLEXPRESS; database=TrainVault; trusted_connection=true; TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1276F7C75");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.JobTitle).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.Phone).HasMaxLength(20);

            entity.HasOne(d => d.Organization).WithMany(p => p.Employees)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Organization");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK__Organiza__CADB0B72D0A59E3D");

            entity.ToTable("Organization");

            entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.OrganizationName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(50);
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.HasKey(e => e.TrainingId).HasName("PK__Training__E8D71DE248646C23");

            entity.Property(e => e.TrainingId).HasColumnName("TrainingID");
            entity.Property(e => e.PlaceOfTraining).HasMaxLength(255);
            entity.Property(e => e.PurposeOfTraining).HasMaxLength(255);

            entity.HasOne(d => d.Organization).WithMany(p => p.Training)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Trainings__Organ__2F10007B");

            entity.HasMany(d => d.Employees).WithMany(p => p.Training)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainingEmployee",
                    r => r.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .HasConstraintName("FK__TrainingE__Emplo__34C8D9D1"),
                    l => l.HasOne<Training>().WithMany()
                        .HasForeignKey("TrainingId")
                        .HasConstraintName("FK__TrainingE__Train__33D4B598"),
                    j =>
                    {
                        j.HasKey("TrainingId", "EmployeeId").HasName("PK__Training__EF7A19732BE47524");
                        j.ToTable("TrainingEmployee");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
