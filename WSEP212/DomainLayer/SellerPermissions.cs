#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
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

        [Key] 
        [Column(Order = 1)]
        public string SellerName{ get; set; }
        
        [Key]
        [Column(Order=2)]
        public int StoreID { get; set; }

        [Key]
        [Column(Order=3)]
        public string GrantorName{ get; set; }
        
        // Only the grantor can update the permissions of the grantee - no need for thread safe collection
        [NotMapped]
        public ConcurrentLinkedList<Permissions> permissionsInStore { get; private set; }

        public string PermissionsInStoreAsJson
        {
            get => JsonConvert.SerializeObject(permissionsInStore);
            set => permissionsInStore = JsonConvert.DeserializeObject<ConcurrentLinkedList<Permissions>>(value);
        }

        public SellerPermissions() { }

        private SellerPermissions(string SellerName, int StoreID, string GrantorName, ConcurrentLinkedList<Permissions> permissionsInStore)
        {
            this.SellerName = SellerName;
            this.StoreID = StoreID;
            this.GrantorName = GrantorName;
            this.permissionsInStore = permissionsInStore;
        }

        public void addToDB()
        {
            SystemDBAccess.Instance.Permissions.Add(this);
            try
            {
                SystemDBAccess.Instance.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
        // Checks that there is no other permission for this seller and store
        // If there is one, return it, else, create new permission
        public static SellerPermissions getSellerPermissions(string SellerName, int StoreID, string GrantorName, ConcurrentLinkedList<Permissions> permissions)
        {
            User seller = UserRepository.Instance.findUserByUserName(SellerName).getValue();
            if (seller.sellerPermissions != null)
            {
                Node<SellerPermissions> sellerPermissions = seller.sellerPermissions.First;
                while(sellerPermissions.Value != null)
                {
                    if (sellerPermissions.Value.StoreID != 0)
                    {
                        if (StoreID == sellerPermissions.Value.StoreID)
                        {
                            return sellerPermissions.Value;
                        }
                    }
                    sellerPermissions = sellerPermissions.Next;
                }
            }
            var sellerPer = new SellerPermissions(SellerName, StoreID, GrantorName, permissions);
            sellerPer.addToDB();
            return sellerPer;
        }

        public int getStoreID()
        {
            return StoreID;
        }

        public bool isStoreOwner()
        {
            return permissionsInStore.Contains(Permissions.AllPermissions);
        }

        public void setPermissions(ConcurrentLinkedList<Permissions> newPer)
        {
            var result = SystemDBAccess.Instance.Permissions.SingleOrDefault(i => i.GrantorName == this.GrantorName && i.SellerName == this.SellerName && i.StoreID == this.StoreID);
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
