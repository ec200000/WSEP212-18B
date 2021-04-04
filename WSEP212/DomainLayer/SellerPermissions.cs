using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SellerPermissions
    {
        public User seller { get; set; }
        public Store store { get; set; }
        public User grantor { get; set; }
        public ConcurrentLinkedList<Permissions> permissionsInStore { get; set; }

        private SellerPermissions(User seller, Store store, User grantor, ConcurrentLinkedList<Permissions> permissionsInStore)
        {
            this.seller = seller;
            this.store = store;
            this.grantor = grantor;
            this.permissionsInStore = permissionsInStore;
        }

        // Checks that there is no other permission for this seller and store
        // If there is one, return it, else, create new permission
        public static SellerPermissions getSellerPermissions(User seller, Store store, User grantor, ConcurrentLinkedList<Permissions> permissions)
        {
            if (seller.sellerPermissions != null)
            {
                foreach (SellerPermissions sellerPermission in seller.sellerPermissions)
                {
                    if (store.Equals(sellerPermission.store))
                    {
                        return sellerPermission;
                    }
                }
            }
            return new SellerPermissions(seller, store, grantor, permissions);
        }

    }
}
