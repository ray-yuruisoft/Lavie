﻿

// ------------------------------------------------------------------------------------------------
// This code was generated by EntityFramework Reverse POCO Generator (http://www.reversepoco.com/).
// Created by Simon Hughes (https://about.me/simon.hughes).
//
// Do not make changes directly to this file - edit the template instead.
//
// The following connection settings were used to generate this file:
//     Configuration file:     "Lavie.Modules.Project.Repositories\App.config"
//     Connection String Name: "ApplicationServices"
//     Connection String:      "Data Source=.;Initial Catalog=FastOA;User Id=sa;password=**zapped**;;Pooling=True;Max Pool Size=200;Min Pool Size=5;"
// ------------------------------------------------------------------------------------------------
// Database Edition       : Enterprise Edition (64-bit)
// Database Engine Edition: Enterprise

// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.5
#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Admin.Repositories
{

    #region Unit of work

    public interface ILavieDbContext : System.IDisposable
    {
        System.Data.Entity.DbSet<Bulletin> Bulletins { get; set; } // Bulletin
        System.Data.Entity.DbSet<Group> Groups { get; set; } // Group
        System.Data.Entity.DbSet<Log> Logs { get; set; } // Log
        System.Data.Entity.DbSet<MobileValidationCode> MobileValidationCodes { get; set; } // MobileValidationCode
        System.Data.Entity.DbSet<Notification> Notifications { get; set; } // Notification
        System.Data.Entity.DbSet<NotificationUser> NotificationUsers { get; set; } // NotificationUser
        System.Data.Entity.DbSet<Permission> Permissions { get; set; } // Permission
        System.Data.Entity.DbSet<Role> Roles { get; set; } // Role
        System.Data.Entity.DbSet<Site> Sites { get; set; } // Site
        System.Data.Entity.DbSet<User> Users { get; set; } // User

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
        System.Data.Entity.Infrastructure.DbChangeTracker ChangeTracker { get; }
        System.Data.Entity.Infrastructure.DbContextConfiguration Configuration { get; }
        System.Data.Entity.Database Database { get; }
        System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity);
        System.Collections.Generic.IEnumerable<System.Data.Entity.Validation.DbEntityValidationResult> GetValidationErrors();
        System.Data.Entity.DbSet Set(System.Type entityType);
        System.Data.Entity.DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();
    }

    #endregion

    #region Database context

    public class LavieDbContext : System.Data.Entity.DbContext, ILavieDbContext
    {
        public System.Data.Entity.DbSet<Bulletin> Bulletins { get; set; } // Bulletin
        public System.Data.Entity.DbSet<Group> Groups { get; set; } // Group
        public System.Data.Entity.DbSet<Log> Logs { get; set; } // Log
        public System.Data.Entity.DbSet<MobileValidationCode> MobileValidationCodes { get; set; } // MobileValidationCode
        public System.Data.Entity.DbSet<Notification> Notifications { get; set; } // Notification
        public System.Data.Entity.DbSet<NotificationUser> NotificationUsers { get; set; } // NotificationUser
        public System.Data.Entity.DbSet<Permission> Permissions { get; set; } // Permission
        public System.Data.Entity.DbSet<Role> Roles { get; set; } // Role
        public System.Data.Entity.DbSet<Site> Sites { get; set; } // Site
        public System.Data.Entity.DbSet<User> Users { get; set; } // User

        static LavieDbContext()
        {
            System.Data.Entity.Database.SetInitializer<LavieDbContext>(null);
        }

        public LavieDbContext()
            : base("Name=ApplicationServices")
        {
        }

        public LavieDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public LavieDbContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
        }

        public LavieDbContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public LavieDbContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new BulletinConfiguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Configurations.Add(new LogConfiguration());
            modelBuilder.Configurations.Add(new MobileValidationCodeConfiguration());
            modelBuilder.Configurations.Add(new NotificationConfiguration());
            modelBuilder.Configurations.Add(new NotificationUserConfiguration());
            modelBuilder.Configurations.Add(new PermissionConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new SiteConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new BulletinConfiguration(schema));
            modelBuilder.Configurations.Add(new GroupConfiguration(schema));
            modelBuilder.Configurations.Add(new LogConfiguration(schema));
            modelBuilder.Configurations.Add(new MobileValidationCodeConfiguration(schema));
            modelBuilder.Configurations.Add(new NotificationConfiguration(schema));
            modelBuilder.Configurations.Add(new NotificationUserConfiguration(schema));
            modelBuilder.Configurations.Add(new PermissionConfiguration(schema));
            modelBuilder.Configurations.Add(new RoleConfiguration(schema));
            modelBuilder.Configurations.Add(new SiteConfiguration(schema));
            modelBuilder.Configurations.Add(new UserConfiguration(schema));
            return modelBuilder;
        }
    }
    #endregion

    #region POCO classes

    // Bulletin
    public class Bulletin
    {
        public System.Guid BulletinID { get; set; } // BulletinID (Primary key)
        public bool IsShow { get; set; } // IsShow
        public string Title { get; set; } // Title (length: 200)
        public string Content { get; set; } // Content (length: 2000)
        public System.DateTime? PublishDate { get; set; } // PublishDate
    }

    // Group
    public class Group
    {
        public System.Guid? ParentID { get; set; } // ParentID
        public System.Guid GroupID { get; set; } // GroupID (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public int Level { get; set; } // Level
        public int DisplayOrder { get; set; } // DisplayOrder
        public bool IsIncludeUser { get; set; } // IsIncludeUser
        public bool IsDisabled { get; set; } // IsDisabled
        public bool IsSystem { get; set; } // IsSystem

        // Reverse navigation

        /// <summary>
        /// Child Permissions (Many-to-Many) mapped by table [GroupPermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Permission> Permissions { get; set; } // Many to many mapping
        /// <summary>
        /// Child Roles (Many-to-Many) mapped by table [GroupRoleRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Role> Roles { get; set; } // Many to many mapping
        /// <summary>
        /// Child Roles (Many-to-Many) mapped by table [GroupRoleLimit]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Role> LimitRoles { get; set; } // Many to many mapping
        /// <summary>
        /// Child Users where [User].[GroupID] point to this entity (FK_User_Group)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<User> Users { get; set; } // User.FK_User_Group

        public Group()
        {
            Users = new System.Collections.Generic.List<User>();
            Permissions = new System.Collections.Generic.List<Permission>();
            Roles = new System.Collections.Generic.List<Role>();
            LimitRoles = new System.Collections.Generic.List<Role>();
        }
    }

    // Log
    public class Log
    {
        public int LogID { get; set; } // LogID (Primary key)
        public int UserID { get; set; } // UserID
        public int TypeID { get; set; } // TypeID
        public string Description { get; set; } // Description (length: 1000)
        public string IP { get; set; } // IP (length: 20)
        public System.DateTime CreationDate { get; set; } // CreationDate
    }

    // MobileValidationCode
    public class MobileValidationCode
    {
        public string Mobile { get; set; } // Mobile (Primary key) (length: 20)
        public string ValidationCode { get; set; } // ValidationCode (length: 20)
        public int TypeID { get; set; } // TypeID
        public System.DateTime CreationDate { get; set; } // CreationDate
        public System.DateTime ExpirationDate { get; set; } // ExpirationDate
        public System.DateTime? FinishVerifyDate { get; set; } // FinishVerifyDate
        public int VerifyTimes { get; set; } // VerifyTimes
        public int MaxVerifyTimes { get; set; } // MaxVerifyTimes
    }

    // Notification
    public class Notification
    {
        public int NotificationID { get; set; } // NotificationID (Primary key)
        public int? FromUserID { get; set; } // FromUserID
        public int? ToUserID { get; set; } // ToUserID
        public System.DateTime CreationDate { get; set; } // CreationDate
        public string Title { get; set; } // Title (length: 100)
        public string Message { get; set; } // Message (length: 1000)
        public string URL { get; set; } // URL (length: 200)

        // Reverse navigation

        /// <summary>
        /// Child NotificationUsers where [NotificationUser].[NotificationID] point to this entity (FK_NotificationUser_Notification)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<NotificationUser> NotificationUsers { get; set; } // NotificationUser.FK_NotificationUser_Notification

        // Foreign keys

        /// <summary>
        /// Parent User pointed by [Notification].([FromUserID]) (FK_Notification_User_FromUserID)
        /// </summary>
        public virtual User FromUser { get; set; } // FK_Notification_User_FromUserID
        /// <summary>
        /// Parent User pointed by [Notification].([ToUserID]) (FK_Notification_User_ToUserID)
        /// </summary>
        public virtual User ToUser { get; set; } // FK_Notification_User_ToUserID

        public Notification()
        {
            NotificationUsers = new System.Collections.Generic.List<NotificationUser>();
        }
    }

    // NotificationUser
    public class NotificationUser
    {
        public int NotificationID { get; set; } // NotificationID (Primary key)
        public int UserID { get; set; } // UserID (Primary key)
        public System.DateTime? ReadTime { get; set; } // ReadTime
        public System.DateTime? DeleteTime { get; set; } // DeleteTime

        // Foreign keys

        /// <summary>
        /// Parent Notification pointed by [NotificationUser].([NotificationID]) (FK_NotificationUser_Notification)
        /// </summary>
        public virtual Notification Notification { get; set; } // FK_NotificationUser_Notification
        /// <summary>
        /// Parent User pointed by [NotificationUser].([UserID]) (FK_NotificationUser_User)
        /// </summary>
        public virtual User User { get; set; } // FK_NotificationUser_User
    }

    // Permission
    public class Permission
    {
        public System.Guid? ParentID { get; set; } // ParentID
        public System.Guid PermissionID { get; set; } // PermissionID (Primary key)
        public string ModuleName { get; set; } // ModuleName (length: 50)
        public string Name { get; set; } // Name (length: 50)
        public int Level { get; set; } // Level
        public int DisplayOrder { get; set; } // DisplayOrder

        // Reverse navigation

        /// <summary>
        /// Child Groups (Many-to-Many) mapped by table [GroupPermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Group> Groups { get; set; } // Many to many mapping
        /// <summary>
        /// Child Roles (Many-to-Many) mapped by table [RolePermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Role> Roles { get; set; } // Many to many mapping
        /// <summary>
        /// Child Users (Many-to-Many) mapped by table [UserPermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<User> Users { get; set; } // Many to many mapping

        public Permission()
        {
            Groups = new System.Collections.Generic.List<Group>();
            Roles = new System.Collections.Generic.List<Role>();
            Users = new System.Collections.Generic.List<User>();
        }
    }

    // Role
    public class Role
    {
        public System.Guid RoleID { get; set; } // RoleID (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public int DisplayOrder { get; set; } // DisplayOrder
        public bool IsSystem { get; set; } // IsSystem

        // Reverse navigation

        /// <summary>
        /// Child Groups (Many-to-Many) mapped by table [GroupRoleRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Group> Groups { get; set; } // Many to many mapping
        /// <summary>
        /// Child Groups (Many-to-Many) mapped by table [GroupRoleLimit]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Group> LimitGroups { get; set; } // Many to many mapping
        /// <summary>
        /// Child Permissions (Many-to-Many) mapped by table [RolePermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Permission> Permissions { get; set; } // Many to many mapping
        /// <summary>
        /// Child Users (Many-to-Many) mapped by table [UserRoleRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<User> Users { get; set; } // Many to many mapping
        /// <summary>
        /// Child Users where [User].[RoleID] point to this entity (FK_User_Role)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<User> Users_RoleID { get; set; } // User.FK_User_Role

        public Role()
        {
            Groups = new System.Collections.Generic.List<Group>();
            Permissions = new System.Collections.Generic.List<Permission>();
            Users = new System.Collections.Generic.List<User>();
            LimitGroups = new System.Collections.Generic.List<Group>();
            Users_RoleID = new System.Collections.Generic.List<User>();
        }
    }

    // Site
    public class Site
    {
        public System.Guid SiteID { get; set; } // SiteID (Primary key)
        public string Name { get; set; } // Name (length: 50)
        public string Host { get; set; } // Host (length: 100)
        public string Title { get; set; } // Title (length: 200)
        public string Keywords { get; set; } // Keywords (length: 200)
        public string Description { get; set; } // Description (length: 500)
        public string Copyright { get; set; } // Copyright (length: 1000)
        public string FaviconURL { get; set; } // FaviconURL (length: 100)
        public string PageTitleSeparator { get; set; } // PageTitleSeparator (length: 50)
    }

    // User
    public class User
    {
        public System.Guid GroupID { get; set; } // GroupID
        public System.Guid? RoleID { get; set; } // RoleID
        public int UserID { get; set; } // UserID (Primary key)
        public string Username { get; set; } // Username (length: 20)
        public string DisplayName { get; set; } // DisplayName (length: 20)
        public string RealName { get; set; } // RealName (length: 20)
        public bool RealNameIsValid { get; set; } // RealNameIsValid
        public string Email { get; set; } // Email (length: 100)
        public bool EmailIsValid { get; set; } // EmailIsValid
        public string Mobile { get; set; } // Mobile (length: 20)
        public bool MobileIsValid { get; set; } // MobileIsValid
        public string Password { get; set; } // Password (length: 100)
        public UserStatus Status { get; set; } // Status
        public System.DateTime CreationDate { get; set; } // CreationDate
        public string HeadURL { get; set; } // HeadURL (length: 200)
        public string LogoURL { get; set; } // LogoURL (length: 200)
        public string Description { get; set; } // Description (length: 4000)
        public string ClientAgent { get; set; } // ClientAgent (length: 100)
        public string Token { get; set; } // Token (length: 50)
        public string WXOpenID { get; set; } // WXOpenID (length: 50)
        public string WXAOpenID { get; set; } // WXAOpenID (length: 50)
        public byte[] RowVersion { get; private set; } // RowVersion (length: 8)

        // Reverse navigation
        /// <summary>
        /// Child Notifications where [Notification].[FromUserID] point to this entity (FK_Notification_User_FromUserID)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Notification> NotificationsFromUser { get; set; } // Notification.FK_Notification_User_FromUserID
        /// <summary>
        /// Child Notifications where [Notification].[ToUserID] point to this entity (FK_Notification_User_ToUserID)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Notification> NotificationsToUser { get; set; } // Notification.FK_Notification_User_ToUserID
        /// <summary>
        /// Child NotificationUsers where [NotificationUser].[UserID] point to this entity (FK_NotificationUser_User)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<NotificationUser> NotificationUsers { get; set; } // NotificationUser.FK_NotificationUser_User

        /// <summary>
        /// Child Permissions (Many-to-Many) mapped by table [UserPermissionRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Permission> Permissions { get; set; } // Many to many mapping
        /// <summary>
        /// Child Roles (Many-to-Many) mapped by table [UserRoleRelationship]
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Role> Roles { get; set; } // Many to many mapping

        // Foreign keys

        /// <summary>
        /// Parent Group pointed by [User].([GroupID]) (FK_User_Group)
        /// </summary>
        public virtual Group Group { get; set; } // FK_User_Group
        /// <summary>
        /// Parent Role pointed by [User].([RoleID]) (FK_User_Role)
        /// </summary>
        public virtual Role Role { get; set; } // FK_User_Role

        public User()
        {
            Permissions = new System.Collections.Generic.List<Permission>();
            Roles = new System.Collections.Generic.List<Role>();
        }
    }

    #endregion

    #region POCO Configuration

    // Bulletin
    public class BulletinConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Bulletin>
    {
        public BulletinConfiguration()
            : this("dbo")
        {
        }

        public BulletinConfiguration(string schema)
        {
            ToTable("Bulletin", schema);
            HasKey(x => x.BulletinID);

            Property(x => x.BulletinID).HasColumnName(@"BulletinID").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.IsShow).HasColumnName(@"IsShow").HasColumnType("bit").IsRequired();
            Property(x => x.Title).HasColumnName(@"Title").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);
            Property(x => x.Content).HasColumnName(@"Content").HasColumnType("nvarchar").IsOptional().HasMaxLength(2000);
            Property(x => x.PublishDate).HasColumnName(@"PublishDate").HasColumnType("datetime2").IsOptional();
        }
    }

    // Group
    public class GroupConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
            : this("dbo")
        {
        }

        public GroupConfiguration(string schema)
        {
            ToTable("Group", schema);
            HasKey(x => x.GroupID);

            Property(x => x.ParentID).HasColumnName(@"ParentID").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.GroupID).HasColumnName(@"GroupID").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.Level).HasColumnName(@"Level").HasColumnType("int").IsRequired();
            Property(x => x.DisplayOrder).HasColumnName(@"DisplayOrder").HasColumnType("int").IsRequired();
            Property(x => x.IsIncludeUser).HasColumnName(@"IsIncludeUser").HasColumnType("bit").IsRequired();
            Property(x => x.IsDisabled).HasColumnName(@"IsDisabled").HasColumnType("bit").IsRequired();
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").HasColumnType("bit").IsRequired();
            HasMany(t => t.Permissions).WithMany(t => t.Groups).Map(m =>
            {
                m.ToTable("GroupPermissionRelationship", "dbo");
                m.MapLeftKey("GroupID");
                m.MapRightKey("PermissionID");
            });
            HasMany(t => t.Roles).WithMany(t => t.Groups).Map(m =>
            {
                m.ToTable("GroupRoleRelationship", "dbo");
                m.MapLeftKey("GroupID");
                m.MapRightKey("RoleID");
            });
            HasMany(t => t.LimitRoles).WithMany(t => t.LimitGroups).Map(m =>
            {
                m.ToTable("GroupRoleLimit", "dbo");
                m.MapLeftKey("GroupID");
                m.MapRightKey("RoleID");
            });
        }
    }

    // Log
    public class LogConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Log>
    {
        public LogConfiguration()
            : this("dbo")
        {
        }

        public LogConfiguration(string schema)
        {
            ToTable("Log", schema);
            HasKey(x => x.LogID);

            Property(x => x.LogID).HasColumnName(@"LogID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.UserID).HasColumnName(@"UserID").HasColumnType("int").IsRequired();
            Property(x => x.TypeID).HasColumnName(@"TypeID").HasColumnType("int").IsRequired();
            Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").IsOptional().HasMaxLength(1000);
            Property(x => x.IP).HasColumnName(@"IP").HasColumnType("nvarchar").IsRequired().HasMaxLength(20);
            Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime2").IsRequired();
        }
    }

    // MobileValidationCode
    public class MobileValidationCodeConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<MobileValidationCode>
    {
        public MobileValidationCodeConfiguration()
            : this("dbo")
        {
        }

        public MobileValidationCodeConfiguration(string schema)
        {
            ToTable("MobileValidationCode", schema);
            HasKey(x => x.Mobile);

            Property(x => x.Mobile).HasColumnName(@"Mobile").HasColumnType("nvarchar").IsRequired().HasMaxLength(20).HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.ValidationCode).HasColumnName(@"ValidationCode").HasColumnType("nvarchar").IsRequired().HasMaxLength(20);
            Property(x => x.TypeID).HasColumnName(@"TypeID").HasColumnType("int").IsRequired();
            Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime2").IsRequired();
            Property(x => x.ExpirationDate).HasColumnName(@"ExpirationDate").HasColumnType("datetime2").IsRequired();
            Property(x => x.FinishVerifyDate).HasColumnName(@"FinishVerifyDate").HasColumnType("datetime2").IsOptional();
            Property(x => x.VerifyTimes).HasColumnName(@"VerifyTimes").HasColumnType("int").IsRequired();
            Property(x => x.MaxVerifyTimes).HasColumnName(@"MaxVerifyTimes").HasColumnType("int").IsRequired();
        }
    }

    // Notification
    public class NotificationConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Notification>
    {
        public NotificationConfiguration()
            : this("dbo")
        {
        }

        public NotificationConfiguration(string schema)
        {
            ToTable("Notification", schema);
            HasKey(x => x.NotificationID);

            Property(x => x.NotificationID).HasColumnName(@"NotificationID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.FromUserID).HasColumnName(@"FromUserID").HasColumnType("int").IsOptional();
            Property(x => x.ToUserID).HasColumnName(@"ToUserID").HasColumnType("int").IsOptional();
            Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime2").IsRequired();
            Property(x => x.Title).HasColumnName(@"Title").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            Property(x => x.Message).HasColumnName(@"Message").HasColumnType("nvarchar").IsRequired().HasMaxLength(1000);
            Property(x => x.URL).HasColumnName(@"URL").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);

            // Foreign keys
            HasOptional(a => a.FromUser).WithMany(b => b.NotificationsFromUser).HasForeignKey(c => c.FromUserID).WillCascadeOnDelete(false); // FK_Notification_User_FromUserID
            HasOptional(a => a.ToUser).WithMany(b => b.NotificationsToUser).HasForeignKey(c => c.ToUserID).WillCascadeOnDelete(false); // FK_Notification_User_ToUserID
        }
    }

    // NotificationUser
    public class NotificationUserConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<NotificationUser>
    {
        public NotificationUserConfiguration()
            : this("dbo")
        {
        }

        public NotificationUserConfiguration(string schema)
        {
            ToTable("NotificationUser", schema);
            HasKey(x => new { x.NotificationID, x.UserID });

            Property(x => x.NotificationID).HasColumnName(@"NotificationID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.UserID).HasColumnName(@"UserID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.ReadTime).HasColumnName(@"ReadTime").HasColumnType("datetime2").IsOptional();
            Property(x => x.DeleteTime).HasColumnName(@"DeleteTime").HasColumnType("datetime2").IsOptional();

            // Foreign keys
            HasRequired(a => a.Notification).WithMany(b => b.NotificationUsers).HasForeignKey(c => c.NotificationID).WillCascadeOnDelete(false); // FK_NotificationUser_Notification
            HasRequired(a => a.User).WithMany(b => b.NotificationUsers).HasForeignKey(c => c.UserID).WillCascadeOnDelete(false); // FK_NotificationUser_User
        }
    }

    // Permission
    public class PermissionConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Permission>
    {
        public PermissionConfiguration()
            : this("dbo")
        {
        }

        public PermissionConfiguration(string schema)
        {
            ToTable("Permission", schema);
            HasKey(x => x.PermissionID);

            Property(x => x.ParentID).HasColumnName(@"ParentID").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.PermissionID).HasColumnName(@"PermissionID").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.ModuleName).HasColumnName(@"ModuleName").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.Level).HasColumnName(@"Level").HasColumnType("int").IsRequired();
            Property(x => x.DisplayOrder).HasColumnName(@"DisplayOrder").HasColumnType("int").IsRequired();
            HasMany(t => t.Roles).WithMany(t => t.Permissions).Map(m =>
            {
                m.ToTable("RolePermissionRelationship", "dbo");
                m.MapLeftKey("PermissionID");
                m.MapRightKey("RoleID");
            });
            HasMany(t => t.Users).WithMany(t => t.Permissions).Map(m =>
            {
                m.ToTable("UserPermissionRelationship", "dbo");
                m.MapLeftKey("PermissionID");
                m.MapRightKey("UserID");
            });
        }
    }

    // Role
    public class RoleConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Role>
    {
        public RoleConfiguration()
            : this("dbo")
        {
        }

        public RoleConfiguration(string schema)
        {
            ToTable("Role", schema);
            HasKey(x => x.RoleID);

            Property(x => x.RoleID).HasColumnName(@"RoleID").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.DisplayOrder).HasColumnName(@"DisplayOrder").HasColumnType("int").IsRequired();
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").HasColumnType("bit").IsRequired();
            HasMany(t => t.Users).WithMany(t => t.Roles).Map(m =>
            {
                m.ToTable("UserRoleRelationship", "dbo");
                m.MapLeftKey("RoleID");
                m.MapRightKey("UserID");
            });
        }
    }

    // Site
    public class SiteConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Site>
    {
        public SiteConfiguration()
            : this("dbo")
        {
        }

        public SiteConfiguration(string schema)
        {
            ToTable("Site", schema);
            HasKey(x => x.SiteID);

            Property(x => x.SiteID).HasColumnName(@"SiteID").HasColumnType("uniqueidentifier").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.Host).HasColumnName(@"Host").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            Property(x => x.Title).HasColumnName(@"Title").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);
            Property(x => x.Keywords).HasColumnName(@"Keywords").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").IsOptional().HasMaxLength(500);
            Property(x => x.Copyright).HasColumnName(@"Copyright").HasColumnType("nvarchar").IsOptional().HasMaxLength(1000);
            Property(x => x.FaviconURL).HasColumnName(@"FaviconURL").HasColumnType("nvarchar").IsOptional().HasMaxLength(100);
            Property(x => x.PageTitleSeparator).HasColumnName(@"PageTitleSeparator").HasColumnType("nvarchar").IsOptional().HasMaxLength(50);
        }
    }

    // User
    public class UserConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<User>
    {
        public UserConfiguration()
            : this("dbo")
        {
        }

        public UserConfiguration(string schema)
        {
            ToTable("User", schema);
            HasKey(x => x.UserID);

            Property(x => x.GroupID).HasColumnName(@"GroupID").HasColumnType("uniqueidentifier").IsRequired();
            Property(x => x.RoleID).HasColumnName(@"RoleID").HasColumnType("uniqueidentifier").IsOptional();
            Property(x => x.UserID).HasColumnName(@"UserID").HasColumnType("int").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.Username).HasColumnName(@"Username").HasColumnType("nvarchar").IsRequired().HasMaxLength(20);
            Property(x => x.DisplayName).HasColumnName(@"DisplayName").HasColumnType("nvarchar").IsOptional().HasMaxLength(20);
            Property(x => x.RealName).HasColumnName(@"RealName").HasColumnType("nvarchar").IsOptional().HasMaxLength(20);
            Property(x => x.RealNameIsValid).HasColumnName(@"RealNameIsValid").HasColumnType("bit").IsRequired();
            Property(x => x.Email).HasColumnName(@"Email").HasColumnType("nvarchar").IsOptional().HasMaxLength(100);
            Property(x => x.EmailIsValid).HasColumnName(@"EmailIsValid").HasColumnType("bit").IsRequired();
            Property(x => x.Mobile).HasColumnName(@"Mobile").HasColumnType("nvarchar").IsOptional().HasMaxLength(20);
            Property(x => x.MobileIsValid).HasColumnName(@"MobileIsValid").HasColumnType("bit").IsRequired();
            Property(x => x.Password).HasColumnName(@"Password").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            Property(x => x.Status).HasColumnName(@"Status").HasColumnType("int").IsRequired();
            Property(x => x.CreationDate).HasColumnName(@"CreationDate").HasColumnType("datetime2").IsRequired();
            Property(x => x.HeadURL).HasColumnName(@"HeadURL").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);
            Property(x => x.LogoURL).HasColumnName(@"LogoURL").HasColumnType("nvarchar").IsOptional().HasMaxLength(200);
            Property(x => x.Description).HasColumnName(@"Description").HasColumnType("nvarchar").IsOptional().HasMaxLength(4000);
            Property(x => x.ClientAgent).HasColumnName(@"ClientAgent").HasColumnType("nvarchar").IsOptional().HasMaxLength(100);
            Property(x => x.Token).HasColumnName(@"Token").HasColumnType("nvarchar").IsOptional().HasMaxLength(50);
            Property(x => x.WXOpenID).HasColumnName(@"WXOpenID").HasColumnType("nvarchar").IsOptional().HasMaxLength(50);
            Property(x => x.WXAOpenID).HasColumnName(@"WXAOpenID").HasColumnType("nvarchar").IsOptional().HasMaxLength(50);
            Property(x => x.RowVersion).HasColumnName(@"RowVersion").HasColumnType("timestamp").IsRequired().IsFixedLength().HasMaxLength(8).IsRowVersion().IsConcurrencyToken().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed);

            // Foreign keys
            HasOptional(a => a.Role).WithMany(b => b.Users_RoleID).HasForeignKey(c => c.RoleID).WillCascadeOnDelete(false); // FK_User_Role
            HasRequired(a => a.Group).WithMany(b => b.Users).HasForeignKey(c => c.GroupID); // FK_User_Group
        }
    }

    #endregion

}
// </auto-generated>

