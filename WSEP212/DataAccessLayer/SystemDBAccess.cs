using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212.DataAccessLayer
{
    public class SystemDBAccess : DbContext
    {
        public static string server { get; set; }
        public static string database { get; set; }
        public static string userID { get; set; }
        public static string password { get; set; }
        
        public static readonly object savelock = new object();
        
        public static bool mock = false;
        private static readonly Lazy<SystemDBAccess> lazy
            = new Lazy<SystemDBAccess>(() => new SystemDBAccess());

        public static SystemDBAccess Instance => lazy.Value;

        public SystemDBAccess() : base($"Server=tcp: {server}.database.windows.net,1433;Database={database};User ID={userID};Password={password};Connection Timeout=30;Trusted_Connection=False;Encrypt=True;PersistSecurityInfo=True;MultipleActiveResultSets=True;")
        {
            Init();
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBAccess>(new MigrateDatabaseToLatestVersion<SystemDBAccess, Configuration>());
        }
        
        public SystemDBAccess(DbConnection connection) : base(connection, false)
        {
            Database.CommandTimeout = 120;
            Database.SetInitializer<SystemDBAccess>(new DropCreateDatabaseAlways());
        }
        
        public void Init() => Database.Initialize(true);

        private class DropCreateDatabaseAlways : DropCreateDatabaseAlways<SystemDBAccess>
        {
            protected override void Seed(SystemDBAccess context)
            {
            }
        }
        
        private SellerPermissions[] listToArray(ConcurrentLinkedList<SellerPermissions> lst)
        {
            SellerPermissions[] arr = new SellerPermissions[lst.size];
            int i = 0;
            Node<SellerPermissions> node = lst.First;
            while(node.Next != null)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
            }
            return arr;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<SellerPermissions>()
                .HasRequired(s => s.seller)
                .WithMany(u => listToArray(u.sellerPermissions))
                .HasForeignKey(se => se.SellerNameRef)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<SellerPermissions>()
                .HasRequired(s => s.grantor)
                .WithMany(u => listToArray(u.sellerPermissions))
                .HasForeignKey(se => se.GrantorNameRef)
                .WillCascadeOnDelete(true);
            modelBuilder.Entity<SellerPermissions>()
                .HasRequired(s => s.store)
                .WithMany(st => st.storeSellersPermissions.Values)
                .HasForeignKey(se => se.StoreIDRef)
                .WillCascadeOnDelete(true);
                modelBuilder.Entity<User>()
                    .HasMany(u => u.sellerPermissions)
                    .WithRequired(s => s.seller)
                    .HasForeignKey(se => se.SellerNameRef)
                    .WillCascadeOnDelete();
                modelBuilder.Entity<User>()
                    .HasMany(u => u.sellerPermissions)
                    .WithOptional(s => s.grantor)
                    .HasForeignKey(se => se.GrantorNameRef)
                    .WillCascadeOnDelete();*/
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemReview> ItemReviewes { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<ShoppingCart> Carts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Authentication> UsersInfo { get; set; }
        public DbSet<PurchaseInvoice> Invoices { get; set; }
        public DbSet<SellerPermissions> Permissions { get; set; }
        public DbSet<UserConnectionManager> DelayedNotifications { get; set; }
        public DbSet<BidInfo> Bids { get; set; }
    }
}