using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using System.Collections.Concurrent;


namespace WSEP212.DomainLayer
{
    public class StoreManagerFacade : IStoreManagerFacade
    {

        private static readonly Lazy<StoreManagerFacade> lazy
        = new Lazy<StoreManagerFacade>(() => new StoreManagerFacade());

        public static StoreManagerFacade Instance
            => lazy.Value;

        private StoreManagerFacade() { }


        public bool addItemToStorage(int storeID, int quantity, String itemName, String description, double price, String category)
        {
            return StoreRepository.Instance.getStore(storeID).addItemToStorage(quantity, itemName, description, price, category) > 0;
        }

        public bool removeItemFromStorage(int storeID, int itemId)
        {
            return StoreRepository.Instance.getStore(storeID).removeItemFromStorage(itemId);
        }

        public bool changeItemQuantity(int storeID, int itemId, int numOfItems)
        {
            return StoreRepository.Instance.getStore(storeID).changeItemQuantity(itemId, numOfItems);
        }

        public bool editItem(int storeID, int itemId, String itemName, String description, double price, String category, int quantity)
        {
            return StoreRepository.Instance.getStore(storeID).editItem(itemId, itemName, description, price, category, quantity);
        }

        public bool addNewStoreSeller(int storeID, String sellerName, int storeId, String grantorName, ConcurrentLinkedList<String> permissionsList)
        {
            ConcurrentLinkedList<Permissions> sellerPerm = new ConcurrentLinkedList<Permissions>();

            if (permissionsList != null)
            {
                Node<String> sellerPermissions = permissionsList.First;
                while (sellerPermissions.Value != null)
                {
                    switch (sellerPermissions.Value)
                    {
                        case "All Permissions":
                            sellerPerm.TryAdd(Permissions.AllPermissions);
                            break;
                        case "Storage Managment":
                            sellerPerm.TryAdd(Permissions.StorageManagment);
                            break;
                        case "Appoint Store Manager":
                            sellerPerm.TryAdd(Permissions.AppointStoreManager);
                            break;
                        case "Appoint Store Owner":
                            sellerPerm.TryAdd(Permissions.AppointStoreOwner);
                            break;
                        case "Remove Store Manager":
                            sellerPerm.TryAdd(Permissions.RemoveStoreManager);
                            break;
                        case "Edit Managment Permissions":
                            sellerPerm.TryAdd(Permissions.EditManagmentPermissions);
                            break;
                        case "Get Officials Information":
                            sellerPerm.TryAdd(Permissions.GetOfficialsInformation);
                            break;
                        case "Get Store Purchase History":
                            sellerPerm.TryAdd(Permissions.GetStorePurchaseHistory);
                            break;
                        default:
                            break;
                    }
                    sellerPermissions = sellerPermissions.Next;
                }
            }
            User seller = UserRepository.Instance.findUserByUserName(sellerName);
            User grantor = UserRepository.Instance.findUserByUserName(grantorName);
            Store store = StoreRepository.Instance.getStore(storeID);
            SellerPermissions sellerPermission = SellerPermissions.getSellerPermissions(seller, store, grantor, sellerPerm);
            return store.addNewStoreSeller(sellerPermission);
        }

        public void addNewPurchase(int storeID, String userName, ConcurrentDictionary<int, int> items, double totalPrice, DateTime dateOfPurchase)
        {
            Store store = StoreRepository.Instance.getStore(storeID);
            PurchaseInfo purchase = new PurchaseInfo(storeID, userName, items, totalPrice, dateOfPurchase);
            store.addNewPurchase(purchase);
        }
    }
}
