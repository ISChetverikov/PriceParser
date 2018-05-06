using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildWorldParser
{
    class Category
    {
        
        public string Name { get; private set; }
        public string Url { get; private set; }
        public int PageCount { get; private set; }
        public string[] ProductsLinks { get; set; }
        public Product[] Products { get; set; }

        public Category(string name, string url, int pageCount)
        {
            Name = name;
            Url = url;
            PageCount = pageCount;
        }
    }
}
