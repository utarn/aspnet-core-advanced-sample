using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Queries.GetAllCategoryQuery
{
    public class GetAllCategoryQuery : IRequest<List<CategoryViewModel>>
    {

        public class GetAllCategoryQueryHandler : IRequestHandler<GetAllCategoryQuery, List<CategoryViewModel>>
        {
            private readonly ApplicationDbContext _context;

            public GetAllCategoryQueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<CategoryViewModel>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
            {
                return await _context.Categories.Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync(cancellationToken);
            }
        }
    }


    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}
