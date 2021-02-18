using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Balda.WebApi.Database
{
    public class BaldaUserDbContext : IdentityDbContext<BaldaUser, IdentityRole<Guid>, Guid>
    {
        public BaldaUserDbContext(DbContextOptions<BaldaUserDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSnakeCaseNamingConvention();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BaldaUser>(b =>
            {
                b.ToTable("balda_user");
                b.HasIndex(u => u.NormalizedEmail)
                    .HasDatabaseName("balda_user_email_idx");

                b.Property(u => u.UserName).IsRequired();
                b.Property(u => u.NormalizedUserName).IsRequired();

                b.HasIndex(u => u.NormalizedUserName)
                    .IsUnique()
                    .HasDatabaseName("balda_user_name_uidx");
            });

            builder.Entity<IdentityRole<Guid>>(b =>
            {
                b.ToTable("balda_role");
                b.HasIndex(r => r.NormalizedName)
                    .IsUnique()
                    .HasDatabaseName("balda_role_uidx");
            });

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("balda_role_claim");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("balda_user_role");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("balda_user_claim");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("balda_user_token");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("balda_user_login");
        }
    }
}