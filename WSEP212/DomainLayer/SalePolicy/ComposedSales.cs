using System;
using System.Collections.Generic;
using System.Text;
using WSEP212.ConcurrentLinkedList;
using WSEP212.ServiceLayer.Result;

namespace WSEP212.DomainLayer
{
    public abstract class ComposedSales : Sale
    {
        public ConcurrentLinkedList<Sale> sales { get; set; }

        public ComposedSales(ConcurrentLinkedList<Sale> sales) : base()
        {
            this.sales = sales;
        }

        public RegularResult addSale(Sale sale)
        {
            if (!sales.Contains(sale))
            {
                sales.TryAdd(sale);
                return new Ok("The Sale Added Successfully");
            }
            return new Failure("The Sale Is Already Included Here");
        }

        public RegularResult removeSale(Sale sale)
        {
            if (sales.Contains(sale))
            {
                sales.Remove(sale, out _);
                return new Ok("The Sale Removed Successfully");
            }
            return new Failure("The Sale Is Not Included Here");
        }
    }
}
