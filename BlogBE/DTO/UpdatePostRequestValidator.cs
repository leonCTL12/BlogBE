using BlogBE.Constants;
using FluentValidation;

namespace BlogBE.DTO;

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequestDto>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(BlogPostConstraint.MaxTitleLength)
            .WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .WithMessage("Content must be at least 10 characters long.");
    }
}