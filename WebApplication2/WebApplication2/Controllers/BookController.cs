using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    public class BookController : Controller
    {
       

        // GET: Book
        public ActionResult Index(string bookGenre, string searchString, string author, string bookPrice) // ,string publishdata, string description
        {
            IEnumerable<BookViewModel> books = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1786/api/");
                //client.BaseAddress = new Uri("http://localhost:20789/api");

                
                var responseTask = client.GetAsync("booksearch");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<BookViewModel>>();
                    readTask.Wait();

                    books = readTask.Result;
                    var bookViewModels = books.ToList();

                    CreateDropdownData(bookViewModels);

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        //if books are db objects, Contains runs on the database, not C#. on DB, Contains maps to SQL LIKE, which is case insensitive
                        //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-search
                        books = bookViewModels.Where(b => b.Title.Contains(searchString));
                    }
                    if (!string.IsNullOrEmpty(bookGenre))
                    {
                        books = bookViewModels.Where(b => b.Genre.Equals(bookGenre));
                    }

                    if (!string.IsNullOrEmpty(author))
                    {
                        books = bookViewModels.Where(b => b.Author.Equals(author));
                    }

                    if (!string.IsNullOrEmpty(bookPrice))
                    {
                        //Change logic to check price range 
                        string[] words = bookPrice.Split(' ');
                        var fromPrice = Convert.ToDecimal(words[0]);
                        var toPrice = Convert.ToDecimal(words[2]);
                        books = bookViewModels.Where(b => b.Price > fromPrice && b.Price < toPrice);

                    }
                }
                else
                {
                    books = Enumerable.Empty<BookViewModel>();
                    ModelState.AddModelError(string.Empty, "Web api error.");
                }
            }
            return View(books);
        }

        private void CreateDropdownData(IEnumerable<BookViewModel> books)
        {
            //Genre
            var allGenres = books.Select(b => b.Genre).Distinct();
            var genreList = new List<string>();
            genreList.AddRange(allGenres);
            ViewBag.bookGenre = new SelectList(genreList);

            //Price
            var priceCategories = new List<string>();
            var priceRangeDictionary = new Dictionary<decimal, decimal>();
            priceRangeDictionary.Add(1, 10);
            priceRangeDictionary.Add(11, 20);
            priceRangeDictionary.Add(21, 30);
            priceRangeDictionary.Add(31, 100);

            foreach (var range in priceRangeDictionary)
            {
                priceCategories.Add($"{range.Key} - {range.Value}");
            }

            ViewBag.bookPrice = new SelectList(priceCategories);

        }

        public ActionResult Welcome(string name, int numTimes = 1)
        {
            ViewBag.Message = "Hello " + name;
            ViewBag.NumTimes = numTimes;

            return View();
        }

    }
}