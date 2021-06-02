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
using WSEP212.DataAccessLayer;
using WSEP212.DomainLayer.ConcurrentLinkedList;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

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
        
        [NotMapped]
        public ConcurrentDictionary<int, BidInfo> bids { get; set; }
        
        // Only the grantor can update the permissions of the grantee - no need for thread safe collection
        [NotMapped]
        public ConcurrentLinkedList<Permissions> permissionsInStore { get; private set; }

        public string PermissionsInStoreAsJson
        {
            get => JsonConvert.SerializeObject(permissionsInStore);
            set => permissionsInStore = JsonConvert.DeserializeObject<ConcurrentLinkedList<Permissions>>(value);
        }
        
        public string BidsAsJson
        {
            get => JsonConvert.SerializeObject(bids);
            set => bids = JsonConvert.DeserializeObject<ConcurrentDictionary<int, BidInfo>>(value);
        }

        public SellerPermissions() { }

        private SellerPermissions(string SellerName, int StoreID, string GrantorName, ConcurrentLinkedList<Permissions> permissionsInStore)
        {
            this.SellerName = SellerName;
            this.StoreID = StoreID;
            this.GrantorName = GrantorName;
            this.permissionsInStore = permissionsInStore;
            this.bids = new ConcurrentDictionary<int, BidInfo>();
        }

        public void addToDB()
        {
            SystemDBAccess.Instance.Permissions.Add(this);
            try
            {
                lock(SystemDBAccess.savelock)
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
                foreach(var sellerPermissions in seller.sellerPermissions)
                {
                    if (sellerPermissions.StoreID != 0)
                    {
                        if (StoreID == sellerPermissions.StoreID)
                        {
                            return sellerPermissions;
                        }
                    }
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
        
        private Permissions[] persListToArray(ConcurrentLinkedList<Permissions> lst)
        {
            Permissions[] arr = new Permissions[lst.size];
            int i = 0;
            Node<Permissions> node = lst.First;
            while(node.Next != null)
            {
                arr[i] = node.Value;
                node = node.Next;
                i++;
            }
            return arr;
        }

        public bool isStoreOwner()
        {
            Permissions[] pers = persListToArray(permissionsInStore);
            return pers.Contains(Permissions.AllPermissions);
        }

        public void setPermissions(ConcurrentLinkedList<Permissions> newPer)
        {
            var result = SystemDBAccess.Instance.Permissions.SingleOrDefault(i => i.GrantorName == this.GrantorName && i.SellerName == this.SellerName && i.StoreID == this.StoreID);
            if (result != null)
            {
                result.permissionsInStore = newPer;
                if(!JToken.DeepEquals(result.PermissionsInStoreAsJson, this.PermissionsInStoreAsJson))
                    result.PermissionsInStoreAsJson = this.PermissionsInStoreAsJson;
                lock(SystemDBAccess.savelock)
                    SystemDBAccess.Instance.SaveChanges();
                this.permissionsInStore = newPer;
            }
        }

        public RegularResult addBid(int itemId, string buyer, double price)
        {
            BidInfo bidInfo = new BidInfo(itemId, buyer, price);
            var result = SystemDBAccess.Instance.Permissions.SingleOrDefault(i => i.GrantorName == this.GrantorName && i.SellerName == this.SellerName && i.StoreID == this.StoreID);
            if (result != null)
            {
                this.bids.TryAdd(bidInfo.bidID, bidInfo);
                if(!JToken.DeepEquals(result.BidsAsJson, this.BidsAsJson))
                    result.BidsAsJson = this.BidsAsJson;
                lock(SystemDBAccess.savelock)
                    SystemDBAccess.Instance.SaveChanges();
                return new Ok("added a new bid");
            }
            return new Failure("bid adding failed");
        }

        private KeyValuePair<int,BidInfo> findBid(int itemId, string buyer)
        {
            foreach (var bid in bids)
            {
                if (bid.Value.itemID == itemId && bid.Value.buyer.Equals(buyer))
                    return bid;
            }
            return new KeyValuePair<int, BidInfo>(-1, null);
        }
        
        public RegularResult removeBid(int itemId, string buyer)
        {
            var result = SystemDBAccess.Instance.Permissions.SingleOrDefault(i => i.GrantorName == this.GrantorName && i.SellerName == this.SellerName && i.StoreID == this.StoreID);
            if (result != null)
            {
                KeyValuePair<int,BidInfo> bidID = findBid(itemId, buyer);
                if (bidID.Key != -1)
                {
                    result.bids.TryRemove(bidID);
                    if(!JToken.DeepEquals(result.BidsAsJson, this.BidsAsJson))
                        result.BidsAsJson = this.BidsAsJson;
                    lock(SystemDBAccess.savelock)
                        SystemDBAccess.Instance.SaveChanges();
                    this.bids = result.bids;
                    return new Ok("removed the bid");
                }
            }
            return new Failure("bid removing failed");
        }
    }
}
