using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using WSEP212.DomainLayer;

namespace WSEP212
{
    public class SystemDBAccess : DbContext
    {
        private static readonly Lazy<SystemDBAccess> lazy
            = new Lazy<SystemDBAccess>(() => new SystemDBAccess());

        public static SystemDBAccess Instance => lazy.Value;
        
        public SystemDBAccess() : base("Server=tcp:wsep212.database.windows.net,1433;Database=wsep212;User ID=wsep212@wsep212;Password=Ab123456;Connection Timeout=30;")
        {
            Init();
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBAccess>(new MigrateDatabaseToLatestVersion<SystemDBAccess, WSEP212.DomainLayer.Configuration>());
        }
        
        public SystemDBAccess(DbConnection connection) : base(connection, false)
        {
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBAccess>(new DropCreateDatabaseAlways());
        }

        public virtual DbSet<Item> Items { get; set; }
        //public virtual DbSet<Store> Stores { get; set; }
       // public virtual DbSet<ShoppingCart> Carts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<PurchaseInvoice> Invoices { get; set; }
        //public virtual DbSet<SellerPermissions> Permissions { get; set; }
        
        public void Init() => Database.Initialize(true);

        private class DropCreateDatabaseAlways : DropCreateDatabaseAlways<SystemDBAccess>
        {
            protected override void Seed(SystemDBAccess context)
            {
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}