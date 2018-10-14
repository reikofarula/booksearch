using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{

    public class BookController : Controller
    {
        // GET: Book
        public ActionResult Index(string bookGenre, string searchString)
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

                    var allGenres = books.Select(b => b.Genre).Distinct();
                    var genreList = new List<string>();
                    genreList.AddRange(allGenres);
                    ViewBag.bookGenre = new SelectList(genreList);

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        //if books are db objects, Contains runs on the database, not C#. on DB, Contains maps to SQL LIKE, which is case insensitive
                        //https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/adding-search
                        books = books.Where(b => b.Title.Contains(searchString));
                    }

                    if (!string.IsNullOrEmpty(bookGenre))
                    {
                        books = books.Where(b => b.Genre.Equals(bookGenre));
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
        public ActionResult Welcome(string name, int numTimes = 1)
        {
            ViewBag.Message = "Hello " + name;
            ViewBag.NumTimes = numTimes;

            return View();
        }

    }
}