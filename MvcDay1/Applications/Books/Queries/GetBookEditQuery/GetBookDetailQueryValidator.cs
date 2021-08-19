using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MvcDay1.Data;

namespace MvcDay1.Applications.Books.Queries.GetBookEditQuery
{
    public class GetBookDetailQueryValidator : AbstractValidator<GetBookEditQuery>
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
}