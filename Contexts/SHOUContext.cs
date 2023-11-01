using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SHOU.Entity;

namespace SHOU.Contexts;

public partial class SHOUContext : DbContext
{
    public SHOUContext()
    {
    }

    public SHOUContext(DbContextOptions<SHOUContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=ADMIN;Initial Catalog=SHOU;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Encrypt=False;Trust Server Certificate=False;Command Timeout=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_comment");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.AtTime).HasColumnType("datetime");
            entity.Property(e => e.Comment1).HasColumnName("Comment");
            entity.Property(e => e.IdParent)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdPost)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdUser)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_image");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdUser)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl).HasMaxLength(250);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_like");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdPost)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdUser)
                .HasMaxLength(36)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_noti");

            entity.ToTable("Notification");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.AtTime).HasColumnType("datetime");
            entity.Property(e => e.IdPost)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdUser)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Type).HasMaxLength(250);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_post");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("Create_at");
            entity.Property(e => e.IdImage)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.IdUser)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Video).HasMaxLength(250);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("id_user");

            entity.Property(e => e.Id)
                .HasMaxLength(36)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.Avatar).HasMaxLength(1000);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(250);
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
