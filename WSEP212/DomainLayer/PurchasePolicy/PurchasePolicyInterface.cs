﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEP212.DomainLayer.PolicyPredicate;
using WSEP212.DomainLayer.PurchaseTypes;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer.PurchasePolicy
{
    public interface PurchasePolicyInterface
    {
        public void supportPurchaseType(PurchaseType purchaseType);
        public void unsupportPurchaseType(PurchaseType purchaseType);
        public Boolean hasPurchaseTypeSupport(PurchaseType purchaseType);
        public int addPurchasePredicate(Predicate<PurchaseDetails> newPredicate, String predDescription);
        public RegularResult removePurchasePredicate(int predicateID);
        public ResultWithValue<int> composePurchasePredicates(int firstPredicateID, int secondPredicateID, PurchasePredicateCompositionType typeOfComposition);
        public ConcurrentDictionary<int, String> getPurchasePredicatesDescriptions();
        public RegularResult approveByPurchasePolicy(PurchaseDetails purchaseDetails);
    }
}