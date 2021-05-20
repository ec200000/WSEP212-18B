﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using WSEP212.DomainLayer.PolicyPredicate;

namespace WSEP212.DomainLayer.SalePolicy
{
    public class ConditionalSale : Sale
    {
        public Sale sale { get; set; }
        public SalePredicate predicate { get; set; }

        public ConditionalSale(Sale sale, SalePredicate predicate) : 
            base("(In Order To Get The Sale: " + sale.ToString() + ", You Must Met The Condition: " + predicate.ToString() + ")")
        {
            this.sale = sale;
            this.predicate = predicate;
        }

        public override ConditionalSale addSaleCondition(SimplePredicate condition, SalePredicateCompositionType compositionType)
        {
            SalePredicate composedPredicate = null;
            switch(compositionType)
            {
                case SalePredicateCompositionType.AndComposition:
                    composedPredicate = new AndPredicates(condition, predicate);
                    break;
                case SalePredicateCompositionType.OrComposition:
                    composedPredicate = new OrPredicates(condition, predicate);
                    break;
            }
            this.predicate = composedPredicate;
            return this;
        }

        public override int getSalePercentageOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            // checks if the predicate is met
            if (this.predicate.applyPrediacte(purchaseDetails))
            {
                return sale.getSalePercentageOnItem(item, purchaseDetails);
            }
            return 0;
        }

        // apply the sale only if the conditional is met and the sale is relevant to this item
        public override double applySaleOnItem(Item item, PurchaseDetails purchaseDetails)
        {
            int salePercentage = getSalePercentageOnItem(item, purchaseDetails);
            return item.price - ((item.price * salePercentage) / 100);
        }
    }
}
