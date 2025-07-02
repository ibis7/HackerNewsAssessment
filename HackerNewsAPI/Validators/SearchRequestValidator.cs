using FluentValidation;
using HackerNewsAPI.Models;

namespace HackerNewsAPI.Validators
{
    public class SearchRequestValidator : AbstractValidator<SearchRequest>
    {
        public SearchRequestValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0).WithMessage("PageNumber cannot be negative");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("SearchTerm should not exceed 100 characters");
        }
    }
}
