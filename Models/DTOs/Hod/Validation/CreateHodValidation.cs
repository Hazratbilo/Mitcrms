using FluentValidation;
using static MITCRMS.Models.DTOs.Hod.CreateHodRequestModel;

namespace MITCRMS.Models.DTOs.Hod.Validation
{
    public class CreateHodValidation : AbstractValidator<CreateHodRequestModel>
    {
        public CreateHodValidation()
        {
            RuleFor(x => x.FirstName).Length(3, 50).NotEmpty().WithMessage("Firstname is required");
            RuleFor(x => x.LastName).Length(3, 50).NotEmpty().WithMessage("Lastname is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.ConfirmPassword).Matches(x => x.PasswordHash).NotEmpty().WithMessage("Confirm password is required");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Date of birth is required");

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
            RuleFor(x => x.YearsOfExperience).NotEmpty().WithMessage("Years of experience required");
        }
    }
}
