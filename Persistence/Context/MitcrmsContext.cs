using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MITCRMS.Models.Entities;

namespace MITCRMS.Persistence.Context
{

        public class MitcrmsContext : DbContext
        {
            public MitcrmsContext(DbContextOptions<MitcrmsContext> options) : base(options)
            {

            }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.Entity<SuperAdmin>()
                .HasOne(sa => sa.User)
                .WithOne(u => u.SuperAdmin)
                .HasForeignKey<SuperAdmin>(sa => sa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                SeedSuperAdminData(builder);


                SeedRoleData(builder);


                builder.Entity<UserRole>()
                    .HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId);

                builder.Entity<UserRole>()
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId);

                builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
    .HasOne(u => u.SuperAdmin)
    .WithOne(t => t.User)
    .HasForeignKey<SuperAdmin>(t => t.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                    .HasOne(u => u.Tutor)
                    .WithOne(t => t.User)
                    .HasForeignKey<Tutor>(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>()
                  .HasOne(u => u.Bursar)
                  .WithOne(b => b.User)
                  .HasForeignKey<Bursar>(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                  .HasOne(u => u.Admin)
                  .WithOne(a => a.User)
                  .HasForeignKey<Admin>(a => a.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                    .HasOne(u => u.Hod)
                    .WithOne(h => h.User)
                    .HasForeignKey<Hod>(h => h.UserId)
                    .OnDelete(DeleteBehavior.Cascade);


                builder.Entity<Report>()
               .HasOne(p => p.Department)
               .WithMany(pd => pd.Reports)
               .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<Department>()
                    .HasMany(d => d.Tutors)
                    .WithOne(a => a.Department)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Department>()
                   .HasMany(d => d.Bursars)
                   .WithOne(a => a.Department)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Department>()
                  .HasMany(d => d.Admins)
                  .WithOne(a => a.Department)
                  .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Department>()
                 .HasOne(d => d.Hod)
                 .WithOne(a => a.Department)
                 .HasForeignKey<Department>(d => d.HodId)
                 .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Bursar>()
                 .HasMany(d => d.Reports)
                 .WithOne(a => a.Bursar)
                 .OnDelete(DeleteBehavior.Cascade);

 

                base.OnModelCreating(builder);
            }

            private static void SeedSuperAdminData(ModelBuilder modelBuilder)
            {
                //var adminRoleId = Guid.NewGuid();
                //var adminUserId = Guid.NewGuid();

                var SuperadminRoleId = new Guid("d2719e67-52f4-4f9c-bdb2-123456789abc");
                var SuperadminUserId = new Guid("c8f2e5ab-9f34-4b97-8b7c-1a5e86c77e42");

                var role = new Role
                {
                    Id = SuperadminRoleId,
                    RoleName = "SuperAdmin",
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc)
                };

                var hasher = new PasswordHasher<object>();
                var passwordHash = hasher.HashPassword(null, "Admin@001");
                var adminUser = new User
                {
                    Id = SuperadminUserId,
                    Email = "Admin001@gmail.com",
                    PasswordHash = "AQAAAAIAAYagAAAAEJjieFsJGM2Xgr+WpuS3juOABbBCvbqSvpym4WzP/SDMuvGz6qH+EFgm19l8SUHUGA==",
                    EmailConfirmed = true,
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc),


                };


                var userRole = new UserRole
                {
                    Id = new Guid("7ad9b1e1-4c23-46a2-b8e4-219ab417f71f"),
                    RoleId = SuperadminRoleId,
                    UserId = SuperadminUserId,
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc)
                };

                var SuperadminProfile = new SuperAdmin
                {
                    Id = new Guid("f0e25b73-7d1a-4c19-8b2f-09a3efb40d12"),
                    FirstName = "SuperAdmin",
                    LastName = "Mitcrms",
                    Address = "Ogun State",
                    PhoneNumber = "+23470456780",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1997, 11, 10), DateTimeKind.Utc),
                    UserId = SuperadminUserId,
                    DateCreated = DateTime.SpecifyKind(new DateTime(2025, 11, 10), DateTimeKind.Utc),
                };

                modelBuilder.Entity<Role>().HasData(role);
                modelBuilder.Entity<SuperAdmin>().HasData(SuperadminProfile);
                modelBuilder.Entity<User>().HasData(adminUser);
                modelBuilder.Entity<UserRole>().HasData(userRole);
            }

            private void SeedRoleData(ModelBuilder modelBuilder)
            {
                var roles = new List<Role>
            {
                new Role
                {
                    Id = new Guid("a45c9e02-1f0b-4e57-b3d8-9b77b4a302be"),
                    RoleName = "Hod",
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc),
                },
                new Role
                {
                    Id = new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a871"),
                    RoleName = "Bursar",
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc),
                },
                       new Role
                {
                    Id = new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a882"),
                    RoleName = "Admin",
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc),
                },
                              new Role
                {
                    Id = new Guid("6e3d4978-dcb0-42ea-9c48-7f6209d4a856"),
                    RoleName = "Tutor",
                    DateCreated = DateTime.SpecifyKind(new DateTime(2026, 1, 10), DateTimeKind.Utc),
                }
            };

                modelBuilder.Entity<Role>().HasData(roles);
            }


            DbSet<SuperAdmin> SuperAdmins => Set<SuperAdmin>();
            DbSet<User> Users => Set<User>();
            DbSet<UserRole> UserRoles => Set<UserRole>();
            DbSet<Tutor> Tutors => Set<Tutor>();
            DbSet<Bursar> Bursars => Set<Bursar>();

            DbSet<Hod> Hods => Set<Hod>();
            DbSet<Department> Departments => Set<Department>();
            DbSet<Report> Reports => Set<Report>();
        DbSet<Admin> Admins => Set<Admin>();
        DbSet<Role> Roles => Set<Role>();
        }
}
