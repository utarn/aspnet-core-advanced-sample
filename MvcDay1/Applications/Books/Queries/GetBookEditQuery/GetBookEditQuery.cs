using MediatR;

using Microsoft.EntityFrameworkCore;

using MvcDay1.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcDay1.Applications.Books.Queries.GetBookEditQuery
{
    public class GetBookEditQuery : IRequest<BookEditViewModel>
    {
        public int BookId { get; set; }

        public class GetBookEditQueryHandler : IRequestHandler<GetBookEditQuery, BookEditViewModel>
        {
            private readonly ApplicationDbContext _context;

            public GetBookEditQueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<BookEditViewModel> Handle(GetBookEditQuery request, CancellationToken cancellationToken)
            {
                return await _context.Books.Include(b => b.Category)
                    .Where(b => b.Id == request.BookId)
                    .Select(b => new BookEditViewModel()
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Price = b.Price,
                        CategoryId = b.CategoryId
                    })
                    .FirstOrDefaultAsync(cancellationToken);

            }
        }
    }
}
