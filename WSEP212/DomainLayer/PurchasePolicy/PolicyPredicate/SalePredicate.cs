﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEP212.DomainLayer
{
    public abstract class SalePredicate : PurchasePredicate
    {
        public SalePredicate(String predicateDescription) : base(predicateDescription) { }
    }
}
