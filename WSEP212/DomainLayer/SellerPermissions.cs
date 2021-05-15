using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using WSEP212.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SellerPermissions
    {
        
        public string SellerNameRef{ get; set; }
        [Key] [Column(Order = 1)] [ForeignKey("SellerNameRef")]
        [JsonIgnore]
        public User seller { get; set; }

        public int StoreIDRef { get; set; }
        [Key]
        [Column(Order=2)]
        [ForeignKey("StoreIDRef")]
        [JsonIgnore]
        public Store store { get; set; }

        public string GrantorNameRef{ get; set; }
        [Key]
        [Column(Order=3)]
        [ForeignKey("GrantorNameRef")]
        [JsonIgnore]
        public User grantor { get; set; }
        // Only the grantor can update the permissions of the grantee - no need for thread safe collection
        public ConcurrentLinkedList<Permissions> permissionsInStore { get; set; }

        public SellerPermissions(){}

        private SellerPermissions(User seller, Store store, User grantor, ConcurrentLinkedList<Permissions> permissionsInStore)
        {
            this.seller = seller;
            this.SellerNameRef = seller.userName;
            this.store = store;
            this.StoreIDRef = store.storeID;
            this.grantor = grantor;
            this.GrantorNameRef = string.Empty;
            if(grantor != null)
                this.GrantorNameRef = grantor.userName;
            this.permissionsInStore = permissionsInStore;
        }

        // Checks that there is no other permission for this seller and store
        // If there is one, return it, else, create new permission
        public static SellerPermissions getSellerPermissions(User seller, Store store, User grantor, ConcurrentLinkedList<Permissions> permissions)
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

        public int getStoreID()
        {
            return store.storeID;
        }

        public bool isStoreOwner()
        {
            return permissionsInStore.Contains(Permissions.AllPermissions);
        }

    }
}
