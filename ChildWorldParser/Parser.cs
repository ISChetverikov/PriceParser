using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;

namespace ChildWorldParser
{
    class Parser
    {
        private ILogger _logger = null;
        /// <summary>
        /// Целевой url
        /// </summary>
        const string CATEGORY_URL = @"https://www.detmir.ru/catalog/index/name/tovari_dlya_malishei/";
        const string BASE_URL = @"https://www.detmir.ru";
        const int PRODUCTS_PER_PAGE = 80;
        
        public Parser(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// парсинг целевого сайта
        /// </summary>
        /// <param name="categoriesArr">Нужные категории</param>
        public IEnumerable<Category> Parse(string[] categoriesArr)
        {
            var result = new List<Category>();

            var categories = GetCategories();
            foreach (var category in categories)
            {
                _logger.Write($"{category.Name}");
                if (!categoriesArr.Contains(category.Name))
                {
                    _logger.WriteLine("\tНе обрабатываем!");
                    continue;
                }

                _logger.WriteLine();
                FillProductsLinks(category);
                FillProducts(category);

                result.Add(category);
            }

            return result;
        }

        /// <summary>
        /// Функция получения категорий с заданого главного url, без заполнения ссылок на продукты и получения
        /// информации о продуктах
        /// </summary>
        /// <returns>категории товаров с базового url</returns>
        private IEnumerable<Category> GetCategories()
        {
            List<Category> categoriesLst = null;

            var htmlDoc = GetPage(CATEGORY_URL);
            var categories = htmlDoc.DocumentNode.SelectNodes("//div[@class='name_padding']");

            if (categories == null)
            {
                throw new Exception("По заданному url не найдены какие-либо категории");
            }

            categoriesLst = new List<Category>(categories.Count);
            foreach (var category in categories)
            {
                var categoryUrl = category.ChildNodes[1].Attributes["href"].Value;
                var nameWithProductsCount = category.ChildNodes[1].InnerHtml;
                
                var match = new Regex(@"(.+?)\s\((\d{1,5})\)", RegexOptions.Multiline).Match(nameWithProductsCount);

                var name = match.Groups[1].Value;
                var productsCount = Double.Parse(match.Groups[2].Value);
                var pageCount = Convert.ToInt32(Math.Ceiling(productsCount / PRODUCTS_PER_PAGE));
                
                categoriesLst.Add(new Category(name, categoryUrl, pageCount));
                // Пока только первая категория!!
                //break; 
            }
            return categoriesLst;
        }

        /// <summary>
        /// Получение ссылок на продукты категорий
        /// </summary>
        /// <param name="categoryUrl">url категории</param>
        /// <returns>продукты категорий</returns>
        private void FillProductsLinks(Category category)
        {
            var productsLinks = new List<string>(category.PageCount * 80);

            for (int i = 1; i <= category.PageCount; i++)
            {
                var fullLink = $"{category.Url}per_page/{PRODUCTS_PER_PAGE}/page/{i}";
                var htmlDoc = GetPage(fullLink);

                var productLinkTags = htmlDoc.DocumentNode.SelectNodes("//div[normalize-space(@class)='news_item']/a");

                _logger.WriteLine($"Обрабатываю страницу {i} из {category.PageCount}");

                foreach (var tag in productLinkTags)
                {
                    productsLinks.Add(tag.Attributes["href"].Value);
                }
            }

            category.ProductsLinks = productsLinks.ToArray();
        }

        private void FillProducts(Category category)
        {
            var products = new List<Product>(category.ProductsLinks.Length);
            
            foreach (var productLink in category.ProductsLinks)
            {
                var product = GetProductInfo($"{BASE_URL}{productLink}");
                if (product == null)
                    continue;
                Console.WriteLine(product);
                products.Add(product);
            }

            category.Products = products.ToArray(); 
        }

        /// <summary>
        /// Получение информации о продукте 
        /// </summary>
        /// <param name="url">ссылка на продукт</param>
        /// <returns>Продукт</returns>
        public Product GetProductInfo(string url)
        {
            var page = GetPage(url);
            var name = page.DocumentNode.SelectSingleNode("//h1[@id='product_details_name']")?.InnerText;
            if (name == null)
                return null;
            var codeNode = page.DocumentNode.SelectSingleNode("//span[@id='product-internal-id']");
            var code = codeNode.InnerText;

            var dirtyVendorCode = codeNode.ParentNode.NextSibling.NextSibling.InnerText;
            var vendorCode = WebUtility.HtmlDecode(dirtyVendorCode).Substring(9); // Отрезаем слово артикул

            var oldPrice = page.DocumentNode.SelectSingleNode("//div[@class='old_price']").InnerText.Trim();
            var newPrice = Regex.Replace(WebUtility.HtmlDecode(page.DocumentNode.SelectSingleNode("//pr[@id='priceId']").InnerHtml), @"\s", "");

            // Если старой цены не было, то спарсится блок с .руб. Тогда старая цена = новой
            double res;
            if (!Double.TryParse(oldPrice, out res))
                oldPrice = newPrice;

            return new Product(name, code, vendorCode, oldPrice, newPrice);
        }

        /// <summary>
        /// Получение страниц по заданному url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HtmlDocument GetPage(string url)
        {
            Thread.Sleep(100);
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            return htmlDoc;
        }
    }
}
