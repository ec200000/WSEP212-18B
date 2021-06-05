using System.Data.Common;
using System.Data.Entity;
using WSEP212.DomainLayer;
using WSEP212.DomainLayer.AuthenticationSystem;
using WSEP212.DomainLayer.PurchaseTypes;

namespace WSEP212.DataAccessLayer
{
    public class DBInterface : DbContext
    {
        public static string server { get; set; }
        public static string database { get; set; }
        public static string userID { get; set; }
        public static string password { get; set; }
        
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemReview> ItemReviewes { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<ShoppingCart> Carts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Authentication> UsersInfo { get; set; }
        public virtual DbSet<PurchaseInvoice> Invoices { get; set; }
        public virtual DbSet<SellerPermissions> Permissions { get; set; }
        public virtual DbSet<UserConnectionManager> DelayedNotifications { get; set; }
        public virtual DbSet<BidInfo> Bids { get; set; }
        
        public DBInterface(string conn) : base(conn){}
        public DBInterface(DbConnection connection, bool b) : base(connection, b) {}
                
    }
}