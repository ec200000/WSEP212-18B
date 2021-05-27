using System;
using System.Data.Common;
using System.Data.Entity;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SystemDBMock : DbContext
    {
        
        private static readonly Lazy<SystemDBMock> lazy
            = new Lazy<SystemDBMock>(() => new SystemDBMock());

        public static SystemDBMock Instance => lazy.Value;
        
        public SystemDBMock() : base("")
        {
            Init();
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBMock>(new MigrateDatabaseToLatestVersion<SystemDBMock, ConfigurationMock>());
        }

        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemReview> ItemReviewes { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<ShoppingCart> Carts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Authentication> UsersInfo { get; set; }
        public virtual DbSet<PurchaseInvoice> Invoices { get; set; }
        public virtual DbSet<SellerPermissions> Permissions { get; set; }
        public virtual DbSet<UserConnectionManager> DelayedNotifications { get; set; }
        
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