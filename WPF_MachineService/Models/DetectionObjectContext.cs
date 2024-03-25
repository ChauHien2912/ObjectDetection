using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WPF_MachineService.Models
{
    public partial class DetectionObjectContext : DbContext
    {
        public DetectionObjectContext()
        {
        }

        public DetectionObjectContext(DbContextOptions<DetectionObjectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=LAPTOP-Q339A538\\SQLEXPRESS;database=DetectionObject;uid=sa;pwd=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProId)
                    .HasName("PK__Product__5BBBEEF55B52BCD0");

                entity.ToTable("Product");

                entity.Property(e => e.ProId)
                    .ValueGeneratedNever()
                    .HasColumnName("proId");

                entity.Property(e => e.Brand)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Ingredient).HasColumnType("text");

                entity.Property(e => e.ProDescription)
                    .HasColumnType("text")
                    .HasColumnName("proDescription");

                entity.Property(e => e.ProName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("proName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
