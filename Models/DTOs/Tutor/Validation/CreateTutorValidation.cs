using FluentValidation;

namespace MITCRMS.Models.DTOs.Tutor.Validation
{
    public class CreateTutorValidation : AbstractValidator<CreateTutorRequestModel>
    {
        public CreateTutorValidation()
        {
            RuleFor(x => x.FirstName).Length(3, 50).NotEmpty().WithMessage("Firstname is required");
            RuleFor(x => x.LastName).Length(3, 50).NotEmpty().WithMessage("Lastname is required");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required");
            RuleFor(x => x.PasswordHash).NotEmpty().WithMessage("Password is required");
            RuleFor(x => x.ConfirmPassword).Matches(x => x.PasswordHash).NotEmpty().WithMessage("Confirm password is required");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(x => x.DateOfBirth).NotEmpty().WithMessage("Date of birth is required");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");


        }
   }
}
