using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ChildWorldParser
{
    class ExcelWriter
    {
        Excel.Application ExcelApp;
        Excel.Workbook WorkBook;
        int tablesIterator = 1;

        public ExcelWriter(int sheetsCount)
        {
            ExcelApp = new Excel.Application();
            //ExcelApp.DisplayAlerts = false;

            ExcelApp.SheetsInNewWorkbook = sheetsCount;
            WorkBook = ExcelApp.Workbooks.Add(System.Reflection.Missing.Value);
        }
        public bool WriteTable(string tableName, Product[] productsList)
        {
            //Таблица.
            Excel.Worksheet WorkSheet = (Excel.Worksheet)WorkBook.Sheets.Item[tablesIterator];
            tablesIterator++;
            WorkSheet.Name = tableName;
            ((Excel.Range)WorkSheet.Columns[1, Type.Missing]).EntireColumn.ColumnWidth = 80;
            ((Excel.Range)WorkSheet.Columns[2, Type.Missing]).EntireColumn.ColumnWidth = 30;
            ((Excel.Range)WorkSheet.Columns[3, Type.Missing]).EntireColumn.ColumnWidth = 30;

            WorkSheet.Cells[1, 1] = "Название";
            WorkSheet.Cells[1, 2] = "Код продукта";
            WorkSheet.Cells[1, 3] = "Артикул";
            WorkSheet.Cells[1, 4] = "Цена";
            WorkSheet.Cells[1, 5] = "Старая цена";
            var header = (Excel.Range)WorkSheet.Range[WorkSheet.Cells[1, 1], WorkSheet.Cells[1, 5]];
            header.Font.Bold = true;

            Console.WriteLine("Таблица");
            for (int i = 0; i < productsList.Length; i++)
            {
                WorkSheet.Cells[i + 2, 1] = productsList.ElementAt(i).Name;
                WorkSheet.Cells[i + 2, 2] = productsList.ElementAt(i).Code;
                WorkSheet.Cells[i + 2, 3] = productsList.ElementAt(i).VendorCode;
                WorkSheet.Cells[i + 2, 4] = productsList.ElementAt(i).NewPrice;
                WorkSheet.Cells[i + 2, 5] = productsList.ElementAt(i).OldPrice;
            }
            

            return true;
        }

        ~ExcelWriter()
        {
            ExcelApp.Visible = true;
            ExcelApp.UserControl = true;

            //WorkBook.Saved = true;
            //var missing = System.Reflection.Missing.Value;
            //ExcelApp.ActiveWorkbook.SaveCopyAs(@"C\Book1.xlsx");



            ExcelApp.Quit();
        }
    }
}
