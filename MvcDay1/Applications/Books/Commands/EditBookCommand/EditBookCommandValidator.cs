using FluentValidation;

namespace MvcDay1.Applications.Books.Commands.EditBookCommand
{
    public class EditBookCommandValidator : AbstractValidator<EditBookCommand>
    {
        public EditBookCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("รหัสหนังสือห้ามว่าง");

            RuleFor(c => c.Title)
                .NotEmpty()
                .WithMessage("ชื่อหนังสือห้ามว่าง");

            RuleFor(c => c.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("ราคาห้ามว่าง")
                .Must(c => c > 0)
                .WithMessage("ราคาไม่ถูกต้อง");

            RuleFor(c => c.CategoryId)
                .NotEmpty()
                .WithMessage("ประเภทหนังสือห้ามว่าง");
        }
    }
}