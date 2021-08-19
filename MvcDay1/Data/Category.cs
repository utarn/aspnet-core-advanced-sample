using System.Collections.Generic;

namespace MvcDay1.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public virtual ICollection<Book> Books { get; set; }

        public Category()
        {
            Books = new List<Book>();
        }
    }
}