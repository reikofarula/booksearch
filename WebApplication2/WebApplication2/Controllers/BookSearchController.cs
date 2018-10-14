using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    [RoutePrefix("api/booksearch")]
    public class BookSearchController : ApiController
    {
        private List<Book> books = new List<Book>();
        private string xmlAllText;
        private List<BookViewModel> bookViewModels;

        public IHttpActionResult Get()
        {
            ExtractXmlData();

            PopulateViewModels();

            if (bookViewModels.Count == 0)
                return NotFound();
            return Ok(bookViewModels);
        }

        private void PopulateViewModels()
        {
            bookViewModels = books.Select(b =>
                new BookViewModel
                {
                    Author = b.Author,
                    Description = b.Description,
                    Genre = b.Genre,
                    Id = b.Id,
                    Price = b.Price,
                    PublishDate = b.PublishDate,
                    Title = b.Title
                }).ToList<BookViewModel>();
        }

        [Route("{title}")]
        public IHttpActionResult Get(string text)
        {
            if(bookViewModels.Count == 0)
                PopulateViewModels();

            //just an example
            var titleExists = bookViewModels.Any(b => b.Title.Equals(text));

            if (!titleExists) return NotFound();
            {
                //var books = bookViewModels.Where(b => b.Title.Equals(text));

                return Ok(bookViewModels.Where(b => b.Title.Equals(text)));
            }

        }
        private void ExtractXmlData()
        {
            xmlAllText = File.ReadAllText(@"C:\Users\rewat\Downloads\Exercise-BookSearch\books.xml");

            //var x = XDocument.Load(@"C:\Users\rewat\Downloads\Exercise-BookSearch\books.xml");
            //var catalog_books = x.Element("catalog").Elements().Descendants("book").Select(b => b);

            var bs = XDocument.Load(@"C:\Users\rewat\Downloads\Exercise-BookSearch\books.xml")
                .Element("catalog")
                .Elements("book")
                .Select(b => b);

            foreach (var xElement in bs)
            {
                var book = new Book()
                {
                    Id = xElement.FirstAttribute.Value,
                    Author = xElement.Element("author")?.Value,
                    Title = xElement.Element("title")?.Value,
                    Genre = xElement.Element("genre")?.Value,
                    Price = xElement.Element("price")?.Value,
                    PublishDate = xElement.Element("publish_date")?.Value,
                    Description = xElement.Element("description")?.Value
                };
                books.Add(book);
            }
        }
    }

    public class Book
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }

        public string Price { get; set; }
        public string PublishDate { get; set; }
        public string Description { get; set; }
    }
}
