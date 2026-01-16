using Microsoft.EntityFrameworkCore;
using OrganizationService.Domain.Organizations;

namespace OrganizationService.Infrastructure.Persistence;

public class OrganizationDbContext(DbContextOptions<OrganizationDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(b =>
        {
            b.ToTable("organizations");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
            b.Property(x => x.Type).HasColumnName("type").IsRequired();
            b.Property(x => x.Status).HasColumnName("status").IsRequired();
            b.Property(x => x.Siret).HasColumnName("siret").HasMaxLength(14);

            b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
            b.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();

            // ? mapping via la propriété
            b.HasMany(x => x.Members)
             .WithOne()
             .HasForeignKey(m => m.OrganizationId)
             .OnDelete(DeleteBehavior.Cascade);

            // ? indiquer à EF d’utiliser le champ privé derrière Members
            b.Navigation(x => x.Members).UsePropertyAccessMode(PropertyAccessMode.Field);
        });


        modelBuilder.Entity<OrganizationMember>(b =>
        {
            b.ToTable("organization_members");
            b.HasKey(x => x.Id);

            b.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
            b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();

            b.Property(x => x.Role).HasColumnName("role").IsRequired();
            b.Property(x => x.Status).HasColumnName("status").IsRequired();

            b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

            b.HasIndex(x => new { x.OrganizationId, x.UserId }).IsUnique();
        });
    }
}
