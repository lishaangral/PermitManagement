using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PemitManagement.Models;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace PemitManagement.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CustomIssuerField> CustomIssuerFields { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<IssuerRole> IssuerRoles { get; set; }

    public virtual DbSet<IssuerType> IssuerTypes { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Permit> Permits { get; set; }

    public virtual DbSet<PermitIssuer> PermitIssuers { get; set; }

    public virtual DbSet<PermitResponse> PermitResponses { get; set; }

    public virtual DbSet<PermitType> PermitTypes { get; set; }

    public virtual DbSet<PermitTypeIssuer> PermitTypeIssuers { get; set; }

    public virtual DbSet<PermitTypeViolation> PermitTypeViolations { get; set; }

    public virtual DbSet<PermitViolation> PermitViolations { get; set; }

    public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<Violation> Violations { get; set; }

    public virtual DbSet<ViolationImage> ViolationImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<CustomIssuerField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("custom_issuer_fields");

            entity.HasIndex(e => e.FieldKey, "field_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValueSql("'0'")
                .HasColumnName("display_order");
            entity.Property(e => e.FieldKey)
                .HasMaxLength(50)
                .HasColumnName("field_key");
            entity.Property(e => e.FieldName)
                .HasMaxLength(100)
                .HasColumnName("field_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.IsRequired)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_required");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employees");

            entity.HasIndex(e => e.EmpNo, "emp_no").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Department)
                .HasMaxLength(100)
                .HasColumnName("department");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasColumnName("designation");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .HasColumnName("email");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(50)
                .HasColumnName("emp_no");
            entity.Property(e => e.EmployeeRole)
                .HasColumnType("enum('Y','N')")
                .HasColumnName("employee_role");
            entity.Property(e => e.LastLogin)
                .HasMaxLength(250)
                .HasColumnName("last_login");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<IssuerRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("issuer_roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsOptional)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_optional");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("role_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<IssuerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("issuer_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsRequired).HasColumnName("is_required");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("locations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.RefineryType)
                .HasColumnType("enum('PR','PNC')")
                .HasColumnName("refinery_type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permissions");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Permit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permits");

            entity.HasIndex(e => e.LocationId, "location_id");

            entity.HasIndex(e => e.PermitTypeId, "permit_type_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgencyName)
                .HasMaxLength(255)
                .HasColumnName("agency_name");
            entity.Property(e => e.ContractWorkerName)
                .HasMaxLength(255)
                .HasColumnName("contract_worker_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(255)
                .HasColumnName("employee_name");
            entity.Property(e => e.ExactLocation)
                .HasMaxLength(255)
                .HasColumnName("exact_location");
            entity.Property(e => e.IncidentCount).HasColumnName("incident_count");
            entity.Property(e => e.IssuerEmployeeId)
                .HasMaxLength(50)
                .HasColumnName("issuer_employee_id");
            entity.Property(e => e.IssuerName)
                .HasMaxLength(255)
                .HasColumnName("issuer_name");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.PermitNumber)
                .HasMaxLength(50)
                .HasColumnName("permit_number");
            entity.Property(e => e.PermitTypeId).HasColumnName("permit_type_id");
            entity.Property(e => e.ReceiverEmployeeId)
                .HasMaxLength(50)
                .HasColumnName("receiver_employee_id");
            entity.Property(e => e.ReceiverName)
                .HasMaxLength(255)
                .HasColumnName("receiver_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.WorkDate).HasColumnName("work_date");
            entity.Property(e => e.WorkOrderNumber)
                .HasMaxLength(50)
                .HasColumnName("work_order_number");
            entity.Property(e => e.WorkTime)
                .HasColumnType("time")
                .HasColumnName("work_time");

            entity.HasOne(d => d.Location).WithMany(p => p.Permits)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permits_ibfk_1");

            entity.HasOne(d => d.PermitType).WithMany(p => p.Permits)
                .HasForeignKey(d => d.PermitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permits_ibfk_2");
        });

        modelBuilder.Entity<PermitIssuer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permit_issuers");

            entity.HasIndex(e => e.IssuerRoleId, "issuer_role_id");

            entity.HasIndex(e => e.PermitId, "permit_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("employee_id");
            entity.Property(e => e.EmployeeName)
                .HasMaxLength(100)
                .HasColumnName("employee_name");
            entity.Property(e => e.IssuerRoleId).HasColumnName("issuer_role_id");
            entity.Property(e => e.PermitId).HasColumnName("permit_id");
        });

        modelBuilder.Entity<PermitResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permit_responses");

            entity.HasIndex(e => e.PermitId, "permit_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.PermitId).HasColumnName("permit_id");
            entity.Property(e => e.Remarks)
                .HasColumnType("text")
                .HasColumnName("remarks");
            entity.Property(e => e.ResponseText)
                .HasColumnType("text")
                .HasColumnName("response_text");

            entity.HasOne(d => d.Permit).WithMany(p => p.PermitResponses)
                .HasForeignKey(d => d.PermitId)
                .HasConstraintName("permit_responses_ibfk_1");
        });

        modelBuilder.Entity<PermitType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permit_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PermitTypeIssuer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permit_type_issuers");

            entity.HasIndex(e => e.IssuerRoleId, "issuer_role_id");

            entity.HasIndex(e => e.PermitTypeId, "permit_type_id");

            entity.HasIndex(e => new { e.PermitTypeId, e.IssuerRoleId }, "unique_permit_issuer").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.IsOptional)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_optional");
            entity.Property(e => e.IssuerRoleId).HasColumnName("issuer_role_id");
            entity.Property(e => e.PermitTypeId).HasColumnName("permit_type_id");
            entity.Property(e => e.Sequence).HasColumnName("sequence");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PermitTypeViolation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("permit_type_violations")
                .UseCollation("utf8mb4_unicode_ci");

            entity.HasIndex(e => e.PermitTypeId, "permit_type_id");

            entity.HasIndex(e => new { e.PermitTypeId, e.ViolationId }, "permit_type_violation_unique").IsUnique();

            entity.HasIndex(e => e.ViolationId, "violation_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.PermitTypeId).HasColumnName("permit_type_id");
            entity.Property(e => e.ViolationId).HasColumnName("violation_id");

            entity.HasOne(d => d.PermitType).WithMany(p => p.PermitTypeViolations)
                .HasForeignKey(d => d.PermitTypeId)
                .HasConstraintName("permit_type_violations_ibfk_1");

            entity.HasOne(d => d.Violation).WithMany(p => p.PermitTypeViolations)
                .HasForeignKey(d => d.ViolationId)
                .HasConstraintName("permit_type_violations_ibfk_2");
        });

        modelBuilder.Entity<PermitViolation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("permit_violations");

            entity.HasIndex(e => e.PermitId, "permit_id");

            entity.HasIndex(e => e.ViolationId, "violation_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActionTaken)
                .HasColumnType("text")
                .HasColumnName("action_taken");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.PermitId).HasColumnName("permit_id");
            entity.Property(e => e.Remarks)
                .HasColumnType("text")
                .HasColumnName("remarks");
            entity.Property(e => e.ViolationId).HasColumnName("violation_id");

            entity.HasOne(d => d.Permit).WithMany(p => p.PermitViolations)
                .HasForeignKey(d => d.PermitId)
                .HasConstraintName("permit_violations_ibfk_1");

            entity.HasOne(d => d.Violation).WithMany(p => p.PermitViolations)
                .HasForeignKey(d => d.ViolationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permit_violations_ibfk_2");
        });

        modelBuilder.Entity<ProjectStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("project_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Priority).HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','in_progress','completed')")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user_permissions");

            entity.HasIndex(e => e.GrantedBy, "granted_by");

            entity.HasIndex(e => e.PermissionId, "permission_id");

            entity.HasIndex(e => new { e.UserId, e.PermissionId }, "user_permission_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GrantedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("granted_at");
            entity.Property(e => e.GrantedBy).HasColumnName("granted_by");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.GrantedByNavigation).WithMany(p => p.UserPermissionGrantedByNavigations)
                .HasForeignKey(d => d.GrantedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("user_permissions_granted_by_fk");

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("user_permissions_permission_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissionUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_permissions_employee_fk");
        });

        modelBuilder.Entity<Violation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("violations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("active");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Severity)
                .HasDefaultValueSql("'medium'")
                .HasColumnType("enum('low','medium','high')")
                .HasColumnName("severity");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ViolationImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("violation_images");

            entity.HasIndex(e => e.PermitViolationId, "permit_violation_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .HasColumnName("image_path");
            entity.Property(e => e.PermitViolationId).HasColumnName("permit_violation_id");

            entity.HasOne(d => d.PermitViolation).WithMany(p => p.ViolationImages)
                .HasForeignKey(d => d.PermitViolationId)
                .HasConstraintName("violation_images_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
