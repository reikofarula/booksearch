using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class BookViewModel
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }

        public decimal Price { get; set; }
        public string PublishDate { get; set; }
        public string Description { get; set; }
    }
}