using System;
using System.Collections.Generic;
using System.Text;

namespace WSEP212.DomainLayer
{
    class SellerPermissions
    {
        public User seller { get; set; }
        public Store store { get; set; }
        public User grantor { get; set; }
        public List<Permissions> permissions { get; set; }

    }
}
