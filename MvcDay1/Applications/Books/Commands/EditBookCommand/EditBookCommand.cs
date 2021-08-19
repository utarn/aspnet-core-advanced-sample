using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Commands.EditBookCommand
{
    public class EditBookCommand : IRequest<bool>
    {
        [Display(Name = "รหัสหนังสือ")]
        public int Id { get; set; }
        [Display(Name = "ชื่อ")]
        public string Title { get; set; } = default!;
        [Display(Name = "ราคา")]
        public decimal Price { get; set; }
        [Display(Name = "ประเภท")]
        public int CategoryId { get; set; }

        public class EditBookCommandHandler : IRequestHandler<EditBookCommand, bool>
        {
            private readonly ApplicationDbContext _context;

            public EditBookCommandHandler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<bool> Handle(EditBookCommand request, CancellationToken cancellationToken)
            {
                var toEdit = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
                if (toEdit != null)
                {
                    toEdit.Title = request.Title;
                    toEdit.Price = request.Price;
                    toEdit.CategoryId = request.CategoryId;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return true;
            }
        }
    }
}
