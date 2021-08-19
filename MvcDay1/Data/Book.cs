using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MvcDay1.Data
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public decimal Price { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
