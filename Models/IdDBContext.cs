using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lightDiskBack.Models
{
    public class IdDBContext : IdentityDbContext<SysUser, SysRole, int>
    {

        public virtual DbSet<SysMenu> SysMenus { get; set; }

        public virtual DbSet<WpFile> wpFile { get; set; }

        public virtual DbSet<DiskFile> DiskFile { get; set; }

        public virtual DbSet<Share> Share { get; set; }

        public virtual DbSet<ShareFile> ShareFile { get; set; }

        public virtual DbSet<Storage> Storage{ get; set; }


        public IdDBContext()
        {
        }

        public IdDBContext(DbContextOptions<IdDBContext> options)
            : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            modelBuilder.HasCharSet("utf8mb4")
                  .UseCollation("utf8mb4_general_ci");

            modelBuilder.Entity<SysUser>(entity =>
            {
                entity.ToTable("aspnetusers");

                entity.Property(e => e.githubId)
                .HasColumnName("github_id");

                entity.HasOne<Storage>(s => s.storage).WithOne(s => s.SysUser)
                .HasForeignKey<Storage>(s => s.userId);
            });

            modelBuilder.Entity<SysRole>(entity => 
            {
                entity.ToTable("aspnetroles");

                entity.HasMany<SysUser>(s => s.User).WithMany(t => t.Role)
                .UsingEntity(j => j.ToTable("aspnetuserroles"));
            });



            modelBuilder.Entity<SysMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId)
                    .HasName("PRIMARY");

                entity.ToTable("sys_menu");

                entity.Property(e => e.MenuId)
                    .HasColumnType("int(11)")
                    .HasColumnName("menu_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DelFlag)
                    .HasMaxLength(255)
                    .HasColumnName("del_flag");

                entity.Property(e => e.MenuName)
                    .HasMaxLength(255)
                    .HasColumnName("menu_name");

                entity.Property(e => e.MenuPerms)
                    .HasMaxLength(255)
                    .HasColumnName("menu_perms")
                    .HasComment("权限标识");

                entity.Property(e => e.MenuType)
                    .HasMaxLength(255)
                    .HasColumnName("menu_type")
                    .HasComment("菜单类型（M目录 C菜单 F按钮）");

                entity.Property(e => e.MenuUrl)
                    .HasMaxLength(255)
                    .HasColumnName("menu_url");

                entity.Property(e => e.ParentId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parent_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Ignore(e => e.ChildMenu);

                entity.HasMany<SysRole>(s => s.SysRoles).WithMany(t => t.SysMenus)
                .UsingEntity(j => j.ToTable("sys_role_menu"));
            });



            modelBuilder.Entity<WpFile>(entity =>
            {
                entity.HasKey(e => e.fileId)
                    .HasName("PRIMARY");

                entity.Property(e => e.fileId)
                   .HasColumnType("int(11)")
                   .HasColumnName("file_id");

                entity.ToTable("wp_file");

                entity.Property(e => e.fileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");


                entity.Property(e => e.isFolder)
                    .HasMaxLength(255)
                    .HasColumnName("is_folder");

                entity.Property(e => e.delFlag)
                    .HasMaxLength(255)
                    .HasColumnName("del_flag");

                entity.Property(e => e.filePath)
                   .HasMaxLength(255)
                    .HasColumnName("file_path");

                entity.Property(e => e.userId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.Property(e => e.diskFileId)
                   .HasColumnType("int(11)")
                   .HasColumnName("disk_file_id");


                entity.HasOne<DiskFile>(s => s.diskFile).WithMany(t => t.wpFileList)
                 .HasForeignKey(c => c.diskFileId); ;

                entity.HasOne<SysUser>(s => s.SysUser).WithMany(t => t.wpFileList)
                .HasForeignKey(c => c.userId);


                entity.Ignore(e => e.ChildFolder);

            });



            modelBuilder.Entity<DiskFile>(entity =>
            {
                entity.HasKey(e => e.diskFileId)
                    .HasName("PRIMARY");

                entity.Property(e => e.diskFileId)
                   .HasColumnType("int(11)")
                   .HasColumnName("disk_file_id");

                entity.ToTable("disk_file");

                entity.Property(e => e.diskFileSize)
                    .HasMaxLength(255)
                    .HasColumnName("disk_file_size");


                entity.Property(e => e.diskFileUrl)
                    .HasMaxLength(255)
                    .HasColumnName("disk_file_url");


                entity.Property(e => e.diskFileType)
                  .HasMaxLength(255)
                  .HasColumnName("disk_file_type");
            });


            modelBuilder.Entity<Share>(entity =>
            {
                entity.HasKey(e => e.shareId)
                    .HasName("PRIMARY");

                entity.Property(e => e.shareId)
                   .HasColumnType("int(11)")
                   .HasColumnName("share_id");

                entity.ToTable("share");

                entity.Property(e => e.endTime)
                    .HasColumnName("end_time");


                entity.Property(e => e.extractionCode)
                    .HasMaxLength(255)
                    .HasColumnName("extraction_code");


                entity.Property(e => e.shareBatchNum)
                  .HasMaxLength(255)
                  .HasColumnName("share_batch_num");

                entity.Property(e => e.shareStatus)
                 .HasMaxLength(255)
                 .HasColumnName("share_status");

                entity.Property(e => e.userId)
                 .HasColumnName("user_id");

            });


            modelBuilder.Entity<ShareFile>(entity =>
            {
                entity.HasKey(e => e.shareFileId)
                    .HasName("PRIMARY");

                entity.Property(e => e.shareFileId)
                   .HasColumnType("int(11)")
                   .HasColumnName("share_file_id");

                entity.ToTable("share_file");

                entity.Property(e => e.shareBatchNum)
                    .HasMaxLength(255)
                    .HasColumnName("share_batch_num");


                entity.Property(e => e.shareFilePath)
                    .HasMaxLength(255)
                    .HasColumnName("share_file_path");


                

                entity.Property(e => e.wpFileName)
                 .HasMaxLength(255)
                 .HasColumnName("wp_file_name");

                entity.Property(e => e.wpIsFolder)
                 .HasColumnName("wp_is_folder");

             

                entity.Property(e => e.diskFileId)
                .HasColumnName("disk_file_id");

                entity.Ignore(e => e.ChildFolder);

            });


            modelBuilder.Entity<Storage>(entity =>
            {
                entity.HasKey(e => e.storageId)
                    .HasName("PRIMARY");

                entity.Property(e => e.storageId)
                   .HasColumnType("int(11)")
                   .HasColumnName("storage_id");

                entity.ToTable("storage");

                entity.Property(e => e.isMember)
                    .HasMaxLength(255)
                    .HasColumnName("is_member");


                entity.Property(e => e.storageSize)
                    .HasColumnName("storage_size");


                entity.Property(e => e.simpleSize)
                 .HasColumnName("simple_size");

                entity.Property(e => e.memberSize)
                 .HasColumnName("member_size");

                entity.Property(e => e.userId)
                .HasColumnName("user_id");


            });

           

        }
    }
}
