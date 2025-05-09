using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlSamurai.Data.Entities;

namespace UrlSamurai.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Urls> Urls { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Urls>(entity =>
        {
            entity.ToTable("urls");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OwnerId).HasColumnName("ownerId");

            entity.HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.Property(e => e.UrlValue).HasColumnName("urlValue").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("createdAt")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ShortId).HasColumnName("shortId").ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.NumericId).HasColumnName("numericId")
                .UseIdentityAlwaysColumn();
            
            entity.HasIndex(e => e.ShortId).IsUnique();
        });
    }
}