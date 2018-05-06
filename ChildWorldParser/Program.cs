using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ChildWorldParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(new UserLogger());
            var categoriesArr = new String[] { "Гигиена и уход" };

            var result = parser.Parse(categoriesArr);

            var excelWriter = new ExcelWriter(result.Count());
            foreach (var category in result)
            {
                if (category.Products == null)
                    continue;
                excelWriter.WriteTable(category.Name, category.Products);

            }

        }
    }
}
