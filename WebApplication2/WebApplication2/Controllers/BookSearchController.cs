using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Xml.Linq;
using WebApplication2.Models;
using LazyCache;

namespace WebApplication2.Controllers
{

    [RoutePrefix("api/booksearch")]
    public class BookSearchController : ApiController
    {
        private List<BookViewModel> books = new List<BookViewModel>();
        private IEnumerable<XElement> booksXElements;

        IAppCache cache = new CachingService();

        public IHttpActionResult Get()
        {
            ExtractXmlData();

            if (books.Count == 0)
                return NotFound();
            return Ok(books);
        }

        private void ExtractXmlData()
        {
            booksXElements = cache.GetOrAdd("latest_xml", () => BooksXElements());

            foreach (var xElement in booksXElements)
            {
                var book = new BookViewModel()
                {
                    Id = xElement.FirstAttribute.Value,
                    Author = xElement.Element("author")?.Value,
                    Title = xElement.Element("title")?.Value,
                    Genre = xElement.Element("genre")?.Value,
                    Price = ConvertPrice(xElement.Element("price")?.Value),
                    PublishDate = xElement.Element("publish_date")?.Value,
                    Description = xElement.Element("description")?.Value
                };
                books.Add(book);
            }
        }

        private IEnumerable<XElement> BooksXElements()
        {
            var booksXElements = XDocument.Load(@"C:\Users\rewat\Downloads\Exercise-BookSearch\books.xml")
                .Element("catalog")
                .Elements("book")
                .Select(b => b);
            return booksXElements;
        }

        private decimal ConvertPrice(string priceStr)
        {
            decimal decimalPrice;
            priceStr = priceStr.Replace(".", ","); 
            if(decimal.TryParse(priceStr, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("sv-SE"),out decimalPrice))
                return decimalPrice;
            return 0m;
        }
    }

   
}
