﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer.PolicyPredicate
{
    public abstract class SalePredicate : PurchasePredicate
    {
        public SalePredicate() {}
        public SalePredicate(String predicateDescription) : base(predicateDescription) { }
    }
}
