using System;
using System.Data.Common;
using System.Data.Entity;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212.DomainLayer
{
    public class SystemDBMock : DBInterface
    {
        
        private static readonly Lazy<SystemDBMock> lazy
            = new Lazy<SystemDBMock>(() => new SystemDBMock());

        public static SystemDBMock Instance => lazy.Value;
        
        public SystemDBMock() : base("Server=tcp:wsep212b18.database.windows.net,1433;Database=wsep212Btest;User ID=wsep212b@wsep212b18;Password=Ab123456;Connection Timeout=30;Trusted_Connection=False;Encrypt=True;PersistSecurityInfo=True;MultipleActiveResultSets=True;")
        {
            Init();
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBMock>(new MigrateDatabaseToLatestVersion<SystemDBMock, ConfigurationMock>());
        }

        public override DbSet<Item> Items { get; set; }
        public override DbSet<ItemReview> ItemReviewes { get; set; }
        public override DbSet<Store> Stores { get; set; }
        public override DbSet<ShoppingCart> Carts { get; set; }
        public override DbSet<User> Users { get; set; }
        public override DbSet<Authentication> UsersInfo { get; set; }
        public override DbSet<PurchaseInvoice> Invoices { get; set; }
        public override DbSet<SellerPermissions> Permissions { get; set; }
        public override DbSet<UserConnectionManager> DelayedNotifications { get; set; }
        public override DbSet<BidInfo> Bids { get; set; }
        
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