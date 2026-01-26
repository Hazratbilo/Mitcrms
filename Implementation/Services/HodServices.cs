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
    public class HodServices : IHodServices
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IHodRepository _hodRepository;
        private readonly IUnitOfWork _unitOfWork;
        ILogger<HodServices> _logger;
        public HodServices(IUserRepository userRepository,
            UserManager<User> userManager,
            IIdentityService identityService,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            IHodRepository hodRepository,
            IUnitOfWork unitOfWork,
            ILogger<HodServices> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _identityService = identityService;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _hodRepository = hodRepository;
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

        public async Task<BaseResponse<bool>> CreateHodAsync(CreateHodRequestModel request)
        {
            var hodExists = await _userRepository.Any(u => u.Email == request.Email);
            if (hodExists)
            {
                _logger.LogError("Hod with email already exist");
                return new BaseResponse<bool>
                {
                    Message = "Hod with email already exist",
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
                var HodUser = new User
                {
                    Email = request.Email,
                    PasswordHash = _identityService.GetPasswordHash(request.PasswordHash)
                };

                var newUser = await _userManager.CreateAsync(HodUser);
                if (newUser == null)
                {
                    _logger.LogError("User Creation unsuccessful");
                    return new BaseResponse<bool>
                    {
                        Message = "User Creation unsuccessful",
                        Status = false
                    };

                }

                var result = await _userManager.AddToRoleAsync(HodUser, "Hod");
                if (!result.Succeeded)
                {
                    _logger.LogError("Unable to add user to roles");
                    return new BaseResponse<bool>
                    {
                        Message = "Unable to add user to roles",
                        Status = false
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(HodUser);


                if (!result.Succeeded)
                {
                    throw new Exception($"Unable to add Hod to roles");
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

                var Hod = new Hod
                {

                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    UserId = HodUser.Id,
                    DateCreated = DateTime.UtcNow,
                    DepartmentId = dept.Id,
                };

                var createHod = await _hodRepository.Add(Hod);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                if (createHod == null)
                {
                    _logger.LogError("Hod couldn't be added");
                    return new BaseResponse<bool>
                    {
                        Message = "Hod couldn't be added",
                        Status = false,
                    };
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Hod added successfully");
                return new BaseResponse<bool>
                {
                    Message = "Hod added successfully",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error  creating hod, rolling back.....");
                return new BaseResponse<bool>
                {
                    Message = "Error  creating hod, rolling back.....",
                    Status = false
                };
            }
        }

        public async Task<BaseResponse<HodDto>> GetHodByIdAsync(Guid hodId, CancellationToken cancellationToken)
        {
            var hod = await _hodRepository.GetHodByIdAsync(hodId);
            if (hod == null)
            {
                _logger.LogError("Hod doesn't exist");
                return new BaseResponse<HodDto>
                {
                    Message = "Hod doesn't exist",
                    Status = false
                };
            }

            _logger.LogInformation("Hod fetched successfully");
            return new BaseResponse<HodDto>
            {
                Message = "Hod fetched successfully",
                Status = true,
                Data = new HodDto
                {
                    Id = hod.Id,
                    FirstName = hod.FirstName,
                    LastName = hod.LastName,
                    FullName = hod.FullName(),
                    Address = hod.Address,
                    DateOfBirth = hod.DateOfBirth,
                    Email = hod.User.Email,
                    PhoneNumber = hod.PhoneNumber,
                    YearsOfExperience = hod.YearsOfExperience,
                    DateCreated = hod.DateCreated,

                }
            };
        }

        public async Task<BaseResponse<IReadOnlyList<HodDto>>> GetHodAsync(CancellationToken cancellationToken)
        {
            var hod = await _hodRepository.GetAllHodAndTheirDepartment();
            if (!hod.Any())
            {
                _logger.LogError("No data found");
                return new BaseResponse<IReadOnlyList<HodDto>>
                {
                    Message = "No data found",
                    Status = false
                };
            }

            return new BaseResponse<IReadOnlyList<HodDto>>
            {
                Message = "Date fetched seuccessfully",
                Status = true,
                Data = hod.Select(r => new HodDto
                {
                    Id = r.Id,
                    FullName = r.FullName(),
                    YearsOfExperience = r.YearsOfExperience,


                }).ToList()
            };
        }
        public async Task<BaseResponse<bool>> DeleteAsync(Guid hodId)
        {

            var getHod = await _hodRepository.Get<Hod>(h => h.Id == hodId);
            if (getHod == null)
            {
                _logger.LogError("Hod coudn't be found");
                return new BaseResponse<bool>
                {
                    Message = "Hod coudn't be found",
                    Status = false
                };
            }
            _hodRepository.Delete<Hod>(getHod);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Hod Deleted Successfully");
            return new BaseResponse<bool>
            {
                Message = "Hod Deleted Successfully",
                Status = true
            };
        }
    }
}