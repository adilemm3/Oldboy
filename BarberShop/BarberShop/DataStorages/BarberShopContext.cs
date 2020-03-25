using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarberShop.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BarberShop.DataStorages
{
    public class BarberShopContext : DbContext
    {
        public BarberShopContext(DbContextOptions<BarberShopContext> options):base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<MasterServices> MastersServices { get; set; }
        public DbSet<ServiceInVisit> ServiceInVisits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MasterServices>().HasKey(c => new { c.MasterId, c.ServiceId });
            modelBuilder.Entity<MasterServices>().HasOne(c => c.Master).WithMany(s => s.MasterServices).HasForeignKey(c => c.MasterId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MasterServices>().HasOne(c => c.Service).WithMany(s => s.MasterServices).HasForeignKey(c => c.ServiceId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ServiceInVisit>().HasOne(c => c.MasterServices).WithMany(s => s.ServicesInVisit).HasForeignKey(c=>new { c.MasterId,c.ServiceId }).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ServiceInVisit>().HasOne(c => c.Visit).WithMany(s => s.ServicesInVisit).HasForeignKey(c=>c.VisitId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Client>().HasKey(x => x.Id);

            string adminRoleName = "admin";
            string userRoleName = "user";

            string phone = "89234163110";
            string adminPassword = "123456";

            Role adminRole = new Role { Id = Guid.NewGuid(), Name = adminRoleName };
            Role userRole = new Role { Id = Guid.NewGuid(), Name = userRoleName };
            User adminUser = new User { Id = adminRole.Id, FullName="Adil",  Phone = phone, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
