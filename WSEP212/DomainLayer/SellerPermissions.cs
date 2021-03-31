using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    public class SellerPermissions
    {
        public User seller { get; set; }
        public Store store { get; set; }
        public User grantor { get; set; }
        // Only the grantor can update the permissions of the grantee - no need for thread safe collection
        public ConcurrentBag<Permissions> permissionsInStore { get; set; }

        private SellerPermissions(User seller, Store store, User grantor, ConcurrentBag<Permissions> permissionsInStore)
        {
            this.seller = seller;
            this.store = store;
            this.grantor = grantor;
            this.permissionsInStore = permissionsInStore;
        }

        // Checks that there is no other permission for this seller and store
        // If there is one, return it, else, create new permission
        public static SellerPermissions getSellerPermissions(User seller, Store store, User grantor, ConcurrentBag<Permissions> permissions)
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
