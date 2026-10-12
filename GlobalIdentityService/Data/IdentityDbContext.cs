using System;
using System.Collections.Generic;
using GlobalIdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalIdentityService.Data;

public partial class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Privilege> Privileges { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePrivilege> RolePrivileges { get; set; }

    public virtual DbSet<SecurityRoleAudit> SecurityRoleAudits { get; set; }

    public virtual DbSet<SecurityUserAudit> SecurityUserAudits { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("application_pkey");

            entity.ToTable("application", "identity");

            entity.HasIndex(e => e.Code, "uq_application_code").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Enabled)
                .HasDefaultValue(true)
                .HasColumnName("enabled");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Privilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("privilege_pkey");

            entity.ToTable("privilege", "identity");

            entity.HasIndex(e => new { e.Name, e.ApplicationId }, "uq_privilege_name_application_id").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.Application).WithMany(p => p.Privileges)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_privilege_application");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role", "identity");

            entity.HasIndex(e => new { e.Name, e.ApplicationId }, "uq_role_name_for_app").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name).HasColumnName("name");

            entity.HasOne(d => d.Application).WithMany(p => p.Roles)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role_application");
        });

        modelBuilder.Entity<RolePrivilege>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_privilege_pkey");

            entity.ToTable("role_privilege", "identity");

            entity.HasIndex(e => new { e.RoleId, e.PrivilegeId }, "uq_role_privilege").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.PrivilegeId).HasColumnName("privilege_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Privilege).WithMany(p => p.RolePrivileges)
                .HasForeignKey(d => d.PrivilegeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role_privilege_privilege");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePrivileges)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role_privilege_role");
        });

        modelBuilder.Entity<SecurityRoleAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("security_role_audit_pkey");

            entity.ToTable("security_role_audit", "identity");

            entity.HasIndex(e => new { e.RoleId, e.CreatedAt }, "ix_security_role_audit_role_id")
                .IsDescending(false, true)
                .HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "true");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.Ip).HasColumnName("ip");
            entity.Property(e => e.Metadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.SecurityRoleAudits)
                .HasForeignKey(d => d.ActorUserId)
                .HasConstraintName("fk_security_role_audit_user");

            entity.HasOne(d => d.Application).WithMany(p => p.SecurityRoleAudits)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_security_role_audit_application");

            entity.HasOne(d => d.Role).WithMany(p => p.SecurityRoleAudits)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_security_role_audit_role");
        });

        modelBuilder.Entity<SecurityUserAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("security_user_audit_pkey");

            entity.ToTable("security_user_audit", "identity");

            entity.HasIndex(e => new { e.TargetUserId, e.CreatedAt }, "ix_security_user_audit_user_id")
                .IsDescending(false, true)
                .HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "true");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ActorUserId).HasColumnName("actor_user_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.Ip).HasColumnName("ip");
            entity.Property(e => e.Metadata)
                .HasDefaultValueSql("'{}'::jsonb")
                .HasColumnType("jsonb")
                .HasColumnName("metadata");
            entity.Property(e => e.TargetUserId).HasColumnName("target_user_id");

            entity.HasOne(d => d.ActorUser).WithMany(p => p.SecurityUserAuditActorUsers)
                .HasForeignKey(d => d.ActorUserId)
                .HasConstraintName("fk_security_user_audit_user_actor_id");

            entity.HasOne(d => d.Application).WithMany(p => p.SecurityUserAudits)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("fk_security_user_audit_application");

            entity.HasOne(d => d.TargetUser).WithMany(p => p.SecurityUserAuditTargetUsers)
                .HasForeignKey(d => d.TargetUserId)
                .HasConstraintName("fk_security_user_audit_user_target_id");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("session_pkey");

            entity.ToTable("session", "identity");

            entity.HasIndex(e => e.RefreshTokenHash, "ix_session_refresh_token_hash")
                .IsUnique()
                .HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "true");

            entity.HasIndex(e => e.UserId, "ix_session_user_id").HasAnnotation("Npgsql:StorageParameter:deduplicate_items", "true");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Ip).HasColumnName("ip");
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(e => e.RefreshTokenHash).HasColumnName("refresh_token_hash");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Application).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_application");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_session_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pkey");

            entity.ToTable("users", "identity");

            entity.HasIndex(e => e.Email, "uq_users_email").IsUnique();

            entity.HasIndex(e => e.Username, "uq_users_username").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AvatarId).HasColumnName("avatar_id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayName).HasColumnName("display_name");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.EmailVerified).HasColumnName("email_verified");
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'active'::text")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username).HasColumnName("username");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_role_pkey");

            entity.ToTable("user_role", "identity");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "uq_user_role_user_id_role_id").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("assigned_at");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role_role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_role_user");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
