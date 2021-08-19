using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Queries.GetBookQuery
{
    public class GetBookQuery : IRequest<List<BookViewModel>>
    {

        public class GetBookQueryHandler : IRequestHandler<GetBookQuery, List<BookViewModel>>
        {
            private readonly ApplicationDbContext _context;

            public GetBookQueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async  Task<List<BookViewModel>> Handle(GetBookQuery request, CancellationToken cancellationToken)
            {
                return await _context.Books.Select(b => new BookViewModel()
                {
                    Id = b.Id,
                    Title = b.Title,
                }).ToListAsync(cancellationToken);
            }
        }
    }

    public class BookViewModel
    {
        [Display(Name = "รหัสหนังสือ")]
        public int Id { get; set; }
        [Display(Name = "ชื่อหนังสือ")]
        public string Title { get; set; }
    }
}
