using Microsoft.AspNetCore.Identity;
using MITCRMS.Contract.Services;
using MITCRMS.Implementation.Repository;
using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Entities;

namespace MITCRMS.Implementation.Services
{
    public class TutorServices : ITutorServices
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IUnitOfWork _unitOfWork;
        ILogger<TutorServices> _logger;
        public TutorServices(IUserRepository userRepository,
            UserManager<User> userManager,
            IIdentityService identityService,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            ITutorRepository tutorRepository,
            IUnitOfWork unitOfWork,
            ILogger<TutorServices> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _identityService = identityService;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _tutorRepository = tutorRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
     
        private static (bool, string?) ValidatePassword(string password)
        {
            // Minimum length of password
            int minLength = 8;

            // Maximum length of password
            int maxLength = 50;

            // Check for null or empty password
            if (string.IsNullOrEmpty(password))
            {
                return (false, "Password cannot be null or empty.");
            }

            // Check length of password
            if (password.Length < minLength || password.Length > maxLength)
            {
                return (false, $"Password must be between {minLength} and {maxLength} characters long.");
            }

            // Check for at least one uppercase letter, one lowercase letter, and one digit
            bool hasUppercase = false;
            bool hasLowercase = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasUppercase = true;
                }
                else if (char.IsLower(c))
                {
                    hasLowercase = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
            }

            if (!hasUppercase || !hasLowercase || !hasDigit)
            {
                return (false, "Password must contain at least one uppercase letter, one lowercase letter, and one digit.");
            }

            // Check for any characters
            string invalidCharacters = @" !""#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            if (password.IndexOfAny(invalidCharacters.ToCharArray()) == -1)
            {
                return (false, "Password must contain one or more characters.");
            }

            // Password is valid
            return (true, null);
        }

        public async Task<BaseResponse<bool>> CreateTutorAsync(CreateTutorRequestModel request)
        {
            var tutorExists = await _userRepository.Any(u => u.Email == request.Email);
            if (tutorExists)
            {
                _logger.LogError("Tutor with email already exist");
                return new BaseResponse<bool>
                {
                    Message = "Tutor with email already exist",
                    Status = false
                };
            }

            if (request.PasswordHash != request.ConfirmPassword) return new BaseResponse<bool>
            {
                Message = "Password doesnt match!",
                Status = false,
            };

            (var passwordResult, var message) = ValidatePassword(request.PasswordHash);
            if (!passwordResult) return new BaseResponse<bool> { Message = message, Status = false };

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var TutorUser = new User
                {
                    Email = request.Email,
                    PasswordHash = _identityService.GetPasswordHash(request.PasswordHash)
                };

                var newUser = await _userManager.CreateAsync(TutorUser);
                if (newUser == null)
                {
                    _logger.LogError("User Creation unsuccessful");
                    return new BaseResponse<bool>
                    {
                        Message = "User Creation unsuccessful",
                        Status = false
                    };

                }

                var result = await _userManager.AddToRoleAsync(TutorUser, "Tutor");
                if (!result.Succeeded)
                {
                    _logger.LogError("Unable to add user to roles");
                    return new BaseResponse<bool>
                    {
                        Message = "Unable to add user to roles",
                        Status = false
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(TutorUser);


                if (!result.Succeeded)
                {
                    throw new Exception($"Unable to add tutor to roles");
                }

                var dept = await _departmentRepository.GetDepartmentById(request.DepartmentId);
                if (dept == null)
                {
                    return new BaseResponse<bool>
                    {
                        Message = "Department doesn't exist",
                        Status = false,
                    };
                }

                if (request == null)
                {
                    return new BaseResponse<bool>
                    {
                        Message = "All fields are required",
                        Status = false,
                    };
                }

                var Tutor = new Tutor
                {

                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    UserId = TutorUser.Id,
                    DateCreated = DateTime.UtcNow,
                    DepartmentId = dept.Id,
                };

                var createTutor = await _tutorRepository.Add(Tutor);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                if (createTutor == null)
                {
                    _logger.LogError("Tutor couldn't be added");
                    return new BaseResponse<bool>
                    {
                        Message = "Tutor couldn't be added",
                        Status = false,
                    };
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Tutor added successfully");
                return new BaseResponse<bool>
                {
                    Message = "Tutor added successfully",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error  creating tutor, rolling back.....");
                return new BaseResponse<bool>
                {
                    Message = "Error  creating tutor, rolling back.....",
                    Status = false
                };
            }
        }

        public async Task<BaseResponse<TutorDto>> GetTutorByIdAsync(Guid tutorId, CancellationToken cancellationToken)
        {
           var tutor = await _tutorRepository.GetTutorByIdAsync(tutorId);
            if (tutor == null)
            {
                _logger.LogError("Tutor doesn't exist");
                return new BaseResponse<TutorDto>
                {
                    Message = "Tutor doesn't exist",
                    Status = false
                };
            }

            _logger.LogInformation("Tutor fetched successfully");
            return new BaseResponse<TutorDto>
            {
                Message = "Tutor fetched successfully", 
                Status = true,
                Data = new TutorDto
                {
                    Id = tutor.Id,
                    FirstName = tutor.FirstName,
                    LastName = tutor.LastName,
                    FullName = tutor.FullName(),
                    Address = tutor.Address,
                    DateOfBirth = tutor.DateOfBirth,
                    Email = tutor.User.Email,
                    PhoneNumber = tutor.PhoneNumber,
                    YearsOfExperience = tutor.YearsOfExperience,
                    DateCreated = tutor.DateCreated,

                }
            };
        }

        public async Task<BaseResponse<IReadOnlyList<TutorDto>>> GetTutorAsync(CancellationToken cancellationToken)
        {
            var tutor = await _tutorRepository.GetAllTutorAndTheirDepartment();
            if (!tutor.Any())
            {
                _logger.LogError("No data found");
                return new BaseResponse<IReadOnlyList<TutorDto>>
                {
                    Message = "No data found",
                    Status = false
                };
            }

            return new BaseResponse<IReadOnlyList<TutorDto>>
            {
                Message = "Date fetched seuccessfully",
                Status = true,
                Data = tutor.Select(r => new TutorDto
                {
                    Id = r.Id,
                    FullName = r.FullName(),
                    YearsOfExperience = r.YearsOfExperience,


                }).ToList()
            };
        }
        public async Task<BaseResponse<bool>> DeleteAsync(Guid tutorId)
        {

            var getTutor = await _tutorRepository.Get<Tutor>(t => t.Id == tutorId);
            if (getTutor == null)
            {
                _logger.LogError("Tutor coudn't be found");
                return new BaseResponse<bool>
                {
                    Message = "Tutor coudn't be found",
                    Status = false
                };
            }
            _tutorRepository.Delete<Tutor>(getTutor);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Tutor Deleted Successfully");
            return new BaseResponse<bool>
            {
                Message = "Tutor Deleted Successfully",
                Status = true
            };
        }

    }
}