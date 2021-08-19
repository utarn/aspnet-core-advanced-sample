using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Queries.GetBookDetailQuery
{
    public class GetBookDetailQueryValidator : AbstractValidator<GetBookDetailQuery>
    {
        public GetBookDetailQueryValidator(ApplicationDbContext context)
        {
            RuleFor(c => c.BookId)
                .MustAsync(async (bookId, token) =>
                {
                    return await context.Books.AnyAsync(b => b.Id == bookId, token);
                })
                .WithMessage("ไม่มีรหัสหนังสือนี้");

        }
    }

    public class GetBookDetailQuery : IRequest<BookDetailViewModel>
    {
        public int BookId { get; set; }

        public class GetBookDetailQueryHandler : IRequestHandler<GetBookDetailQuery, BookDetailViewModel>
        {
            private readonly ApplicationDbContext _context;

            public GetBookDetailQueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<BookDetailViewModel> Handle(GetBookDetailQuery request, CancellationToken cancellationToken)
            {
                return await _context.Books.Include(b => b.Category)
                    .Where(b => b.Id == request.BookId)
                    .Select(b => new BookDetailViewModel()
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Price = b.Price,
                        Category = b.Category.Name
                    })
                    .FirstOrDefaultAsync(cancellationToken);

            }
        }
    }

    public class BookDetailViewModel
    {
        [Display(Name = "รหัสหนังสือ")]
        public int Id { get; set; }
        [Display(Name = "ชื่อ")]
        public string Title { get; set; } = default!;
        [Display(Name = "ราคา")]
        public decimal Price { get; set; }
        [Display(Name = "ประเภท")]
        public string Category { get; set; }
    }
}
