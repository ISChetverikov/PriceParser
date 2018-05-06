using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildWorldParser
{
    class Product
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string VendorCode { get; private set; }
        public string OldPrice { get; private set; }
        public string NewPrice { get; private set; }

        public  Product(string name, string code, string vendorCode, string oldPrice, string newPrice)
        {
            Name = name;
            Code = code;
            VendorCode = vendorCode;
            OldPrice = oldPrice;
            NewPrice = newPrice;
        }

        public override string ToString()
        {
            return $"{Name}\t{Code}\t{VendorCode}\t{OldPrice}-{NewPrice}";
        }
    }
}
