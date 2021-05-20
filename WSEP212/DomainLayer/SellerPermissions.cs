#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WSEP212.ConcurrentLinkedList;
using WSEP212.DomainLayer.ConcurrentLinkedList;

namespace WSEP212.DomainLayer
{
    public class SellerPermissions
    {
        [JsonIgnore]
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        
        [Key] 
        [Column(Order = 1)]
        public string SellerNameRef{ get; set; }
        [ForeignKey("SellerNameRef")]
        [JsonIgnore]
        public User seller { get; set; }
        [Key]
        [Column(Order=2)]
        public int StoreIDRef { get; set; }
        
        
        [ForeignKey("StoreIDRef")]
        [JsonIgnore]
        public Store store { get; set; }
        [Key]
        [Column(Order=3)]
        public string GrantorNameRef{ get; set; }
        
        
        [ForeignKey("GrantorNameRef")]
        [JsonIgnore]
        public User grantor { get; set; }
        // Only the grantor can update the permissions of the grantee - no need for thread safe collection
        [NotMapped]
        public ConcurrentLinkedList<Permissions> permissionsInStore { get; private set; }

        public string PermissionsInStoreAsJson
        {
            get => JsonConvert.SerializeObject(permissionsInStore);
            set => permissionsInStore = JsonConvert.DeserializeObject<ConcurrentLinkedList<Permissions>>(value);
        }

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
            SystemDBAccess.Instance.Permissions.Add(this);
            SystemDBAccess.Instance.SaveChanges();
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

        public void setPermissions(ConcurrentLinkedList<Permissions> newPer)
        {
            var result = SystemDBAccess.Instance.Permissions.SingleOrDefault(i => i.SellerNameRef == this.SellerNameRef && i.GrantorNameRef == this.GrantorNameRef && i.StoreIDRef == this.StoreIDRef);
            if (result != null)
            {
                result.permissionsInStore = newPer;
                if(!JToken.DeepEquals(result.PermissionsInStoreAsJson, this.PermissionsInStoreAsJson))
                    result.PermissionsInStoreAsJson = this.PermissionsInStoreAsJson;
                SystemDBAccess.Instance.SaveChanges();
                this.permissionsInStore = newPer;
            }
        }

    }
}
