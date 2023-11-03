using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RizenSoftApiV2.Models
{
    public partial class RizenSoftDBContext : DbContext
	{

		public RizenSoftDBContext(DbContextOptions<RizenSoftDBContext> options) : base(options)
        {
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Address> Address { get; set; }

        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.ExpiryDate);

                entity.Property(e => e.TokenHash)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.TokenSalt)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Ts)
                    .HasColumnName("TS");

                entity.ToTable("RefreshToken");
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.AddressId)
                    .IsRequired();

                entity.Property(e => e.AddressLine1)
                    .IsRequired();

                entity.Property(e => e.AddressLine2);

                entity.Property(e => e.Suburb)
                    .IsRequired();

                entity.Property(e => e.City)
                    .IsRequired();

                entity.Property(e => e.Province)
                    .IsRequired();

                entity.Property(e => e.Country)
                    .IsRequired();

                entity.Property(e => e.PostalCode)
                    .IsRequired();

                entity.ToTable("Address");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ConfirmPassword)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.IdNumber)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.AddressId)
                    .IsRequired();

                entity.Property(e => e.DateOfBirth);

                entity.ToTable("User");
            });

            OnModelCreatingPartial(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}

