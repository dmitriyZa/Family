using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<FamilyMember> FamilyMembers { get; set; }
    public DbSet<Relationship> Relationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.FamilyMember)
            .WithMany(fm => fm.Relationships)
            .HasForeignKey(r => r.FamilyMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.RelatedMember)
            .WithMany()
            .HasForeignKey(r => r.RelatedMemberId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

