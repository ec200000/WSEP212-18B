using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;

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
                Node<SellerPermissions> sellerPermissions = seller.sellerPermissions.First;
                while(sellerPermissions.Value != null)
                {
                    if (sellerPermissions.Value.store != null)
                    {
                        if (store.Equals(sellerPermissions.Value.store))
                        {
                            return sellerPermissions.Value;
                        }
                    }
                    sellerPermissions = sellerPermissions.Next;
                }
            }
            return new SellerPermissions(seller, store, grantor, permissions);
        }

    }
}
