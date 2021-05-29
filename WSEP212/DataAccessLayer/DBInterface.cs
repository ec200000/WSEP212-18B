using System.Data.Common;
using System.Data.Entity;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212.DataAccessLayer
{
    public class DBInterface : DbContext
    {
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
        
        public DBInterface(string conn) : base(conn){}
        public DBInterface(DbConnection connection, bool b) : base(connection, b) {}
                
    }
}