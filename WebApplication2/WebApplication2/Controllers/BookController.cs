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
        private static readonly List<string> toIgnore = new List<string> { ",", ".", "/", "\\", "-", "=" };
        private static readonly char[] charsToTrim = new char[] { ',', '.', '/', '\\', '-', '=' };
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
                        var searchStringsTrimmed = TrimSearchKeyword(searchString);

                        var idTitlestringDictionary = new Dictionary<string, string[]>();
                        books.ForEach(b => idTitlestringDictionary.Add(b.Id, b.Title.ToLower().Split(' ')));
                       
                        books = GetMatchedItems(idTitlestringDictionary, searchStringsTrimmed.ToArray(), books);
                    }

                    if (!string.IsNullOrEmpty(author))
                    {
                        var authorStringsTrimmed = TrimSearchKeyword(author);
                       
                        var idAuthorDictionary = new Dictionary<string, string[]>();
                        foreach (var b in books)
                        {
                            char[] toTrim = { ',' };
                            var nameSplit = b.Author.ToLower().Split(' ');
                            var nameArray = new List<string>();
                            nameSplit.ForEach(n => { nameArray.Add(n.TrimEnd(toTrim));});
                             
                            idAuthorDictionary.Add(b.Id, nameArray.ToArray());
                        }
                        books = GetMatchedItems(idAuthorDictionary, authorStringsTrimmed.ToArray(), books);
                    }

                    if (!string.IsNullOrEmpty(bookGenre))
                    {
                        books = books.Where(b => b.Genre.Equals(bookGenre)).ToList();
                    }

                    if (!string.IsNullOrEmpty(bookPrice))
                    {
                        var words = bookPrice.Split(' ');
                        char[] toTrim = {'€'};
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

        private HashSet<string> TrimSearchKeyword(string searchString)
        {
            var serachStrings = searchString.ToLower().Split(' ').ToList();
            serachStrings.RemoveAll(s => toIgnore.Contains(s));

            var searchStringsTrimmed = new HashSet<string>();
            serachStrings.ForEach(ss => searchStringsTrimmed.Add(ss.TrimEnd(charsToTrim)));
            return searchStringsTrimmed;
        }

        private static List<BookViewModel> GetMatchedItems(Dictionary<string, string[]> idBookstringDictionary,
            string[] searchStringArray, IList<BookViewModel> books)
        {
            var matchedTitles = new List<string>();
            foreach (var book in idBookstringDictionary)
            {
                var matchCount = 0;
                var numberOfStrings = searchStringArray.Length;
                for (var index = 0; index < numberOfStrings; index++)
                {
                    if (book.Value.Any(word => word.Contains(searchStringArray[index])))
                        matchCount++;
                    if (matchCount == numberOfStrings)
                        matchedTitles.Add(book.Key);
                }
            }
            return books.Where(b => matchedTitles.Contains(b.Id)).ToList();
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
                    ? $"€{range.Key} - "
                    : $"€{range.Key} - €{range.Value}");
            }
            ViewBag.bookPrice = new SelectList(priceCategories);

        }

    }
}