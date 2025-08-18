using FluentValidation;

namespace BlogBE.DTO;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequestDto>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.")
            .MaximumLength(500)
            .WithMessage("Content must not exceed 500 characters.");

        RuleFor(x => x.PostId)
            .GreaterThan(0)
            .WithMessage("PostId must be a positive integer.");
    }
}