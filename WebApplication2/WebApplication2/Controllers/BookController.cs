using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using WebApplication2.Models;
using WebGrease.Css.Extensions;

namespace WebApplication2.Controllers
{

    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index(string bookGenre, string searchString, string author, string bookPrice) // string description, string publishdate
        {
            IList<BookViewModel> books = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:1786/api/");
                
                var responseTask = client.GetAsync("booksearch");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<BookViewModel>>();
                    readTask.Wait();

                    books = readTask.Result;

                    CreateDropdownData(books);

                     if (!string.IsNullOrEmpty(searchString))
                    {
                        //if books are db objects, Contains runs on the database, not C#. on DB, Contains maps to SQL LIKE, which is case insensitive
                        //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-search

                        //Split every book's title into string array
                        var idTitlestrings = new Dictionary<string, string[]>();
                        books.ForEach(b => idTitlestrings.Add(b.Id, b.Title.ToLower().Split(' ')));

                        //Split searchstring into string array
                        var searchStringArray = searchString.ToLower().Split(' ');

                        var matchedTitles = new List<string>();
                        foreach (var bookTitle in idTitlestrings)
                        {
                            var matchCount = 0;
                            var numberOfStrings = searchStringArray.Length;
                            for (var index = 0; index < numberOfStrings; index++)
                            {
                                if (bookTitle.Value.Any(word => word.Contains(searchStringArray[index])))
                                    matchCount++;
                                if(matchCount == numberOfStrings)
                                    matchedTitles.Add(bookTitle.Key);
                            }
                           
                        }

                        books = books.Where(b => matchedTitles.Contains(b.Id)).ToList();

                    }
                    if (!string.IsNullOrEmpty(bookGenre))
                    {
                        books = books.Where(b => b.Genre.Equals(bookGenre)).ToList();
                    }

                    if (!string.IsNullOrEmpty(author))
                    {
                        books = books.Where(b => b.Author.Equals(author)).ToList();
                    }

                    if (!string.IsNullOrEmpty(bookPrice))
                    {
                        var words = bookPrice.Split(' ');
                        char[] toTrim = {'$'};
                        var fromPrice = Convert.ToDecimal(words[0].Trim(toTrim));
                        var toPrice = words.Length == 2 ? -99m :  Convert.ToDecimal(words[2].Trim(toTrim));
                        books = toPrice < 0 ? books.Where(b => b.Price > fromPrice).ToList() : books.Where(b => b.Price > fromPrice && b.Price < toPrice).ToList();
                    }
                }
                else
                {
                    books = Enumerable.Empty<BookViewModel>().ToList();
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
            var priceRangeDictionary = new Dictionary<string, string>();
            priceRangeDictionary.Add("1", "10");
            priceRangeDictionary.Add("11", "20");
            priceRangeDictionary.Add("21", "30");
            priceRangeDictionary.Add("31", "");

            foreach (var range in priceRangeDictionary)
            {
                priceCategories.Add(string.IsNullOrEmpty(range.Value)
                    ? $"${range.Key} - "
                    : $"${range.Key} - ${range.Value}");
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