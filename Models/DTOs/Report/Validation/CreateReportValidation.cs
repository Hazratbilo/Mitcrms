using FluentValidation;

namespace MITCRMS.Models.DTOs.Report.Validation
{
    public class CreateReportValidation : AbstractValidator<CreateReportRequestModel>
    {
        public CreateReportValidation()
        {
            RuleFor(x => x.Hod).NotEmpty().WithMessage("Hod is required");
            RuleFor(x => x.Tutor).NotEmpty().WithMessage("Tutor is required");
            RuleFor(x => x.Bursar).NotEmpty().WithMessage("Hod is required");
            RuleFor(x => x.Admin).NotEmpty().WithMessage("Tutor is required");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required");
            RuleFor(x => x.Tittle).NotEmpty().WithMessage("Tittle is required");
        }
    }
}