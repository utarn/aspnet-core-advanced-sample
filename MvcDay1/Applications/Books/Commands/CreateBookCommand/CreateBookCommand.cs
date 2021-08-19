using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Commands.CreateBookCommand
{
    public class CreateBookCommand : IRequest<bool>
    {
        [Display(Name = "ชื่อ")]
        public string Title { get; set; } = default!;
        [Display(Name = "ราคา")]
        public decimal Price { get; set; }
        [Display(Name = "ประเภท")]
        public int CategoryId { get; set; }

        public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, bool>
        {
            private readonly ApplicationDbContext _context;

            public CreateBookCommandHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(CreateBookCommand request, CancellationToken cancellationToken)
            {
                var newBook = new Book()
                {
                    Title = request.Title,
                    Price = request.Price,
                    CategoryId = request.CategoryId
                };
                await _context.Books.AddAsync(newBook, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
