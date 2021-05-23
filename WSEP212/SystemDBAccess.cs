using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212
{
    public class SystemDBAccess : DbContext
    {
        private static readonly Lazy<SystemDBAccess> lazy
            = new Lazy<SystemDBAccess>(() => new SystemDBAccess());

        public static SystemDBAccess Instance => lazy.Value;
        
        public SystemDBAccess() : base("Server=tcp:wsep212b.database.windows.net,1433;Database=wsep212B;User ID=wsep212@wsep212B;Password=Ab123456;Connection Timeout=30;Trusted_Connection=False;Encrypt=True;PersistSecurityInfo=True;MultipleActiveResultSets=True;")
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
    }
}