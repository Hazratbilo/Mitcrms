using Microsoft.AspNetCore.Identity;
using MITCRMS.Contract.Services;
using MITCRMS.Implementation.Repository;
using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Entities;

namespace MITCRMS.Implementation.Services
{
    public class BursarServices : IBursarServices
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IBursarRepository _bursarRepository;
        private readonly IUnitOfWork _unitOfWork;
        ILogger<BursarServices> _logger;
        public BursarServices(IUserRepository userRepository,
            UserManager<User> userManager,
            IIdentityService identityService,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            IBursarRepository bursarRepository,
            IUnitOfWork unitOfWork,
            ILogger<BursarServices> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _identityService = identityService;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _bursarRepository = bursarRepository;
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

        public async Task<BaseResponse<bool>> CreateBursarAsync(CreateBursarRequestModel request)
        {
            var bursarExists = await _userRepository.Any(u => u.Email == request.Email);
            if (bursarExists)
            {
                _logger.LogError("Bursar with email already exist");
                return new BaseResponse<bool>
                {
                    Message = "Bursar with email already exist",
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
                var BursarUser = new User
                {
                    Email = request.Email,
                    PasswordHash = _identityService.GetPasswordHash(request.PasswordHash)
                };

                var newUser = await _userManager.CreateAsync(BursarUser);
                if (newUser == null)
                {
                    _logger.LogError("User Creation unsuccessful");
                    return new BaseResponse<bool>
                    {
                        Message = "User Creation unsuccessful",
                        Status = false
                    };

                }

                var result = await _userManager.AddToRoleAsync(BursarUser, "Bursar");
                if (!result.Succeeded)
                {
                    _logger.LogError("Unable to add user to roles");
                    return new BaseResponse<bool>
                    {
                        Message = "Unable to add user to roles",
                        Status = false
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(BursarUser);


                if (!result.Succeeded)
                {
                    throw new Exception($"Unable to add bursar to roles");
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

                var Bursar = new Bursar
                {

                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    UserId = BursarUser.Id,
                    DateCreated = DateTime.UtcNow,
                    DepartmentId = dept.Id,
                };

                var createBursar = await _bursarRepository.Add(Bursar);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                if (createBursar == null)
                {
                    _logger.LogError("Bursar couldn't be added");
                    return new BaseResponse<bool>
                    {
                        Message = "Bursar couldn't be added",
                        Status = false,
                    };
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Bursar added successfully");
                return new BaseResponse<bool>
                {
                    Message = "Bursar added successfully",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error  creating bursar, rolling back.....");
                return new BaseResponse<bool>
                {
                    Message = "Error  creating bursar, rolling back.....",
                    Status = false
                };
            }
        }

        public async Task<BaseResponse<BursarDto>> GetBursarByIdAsync(Guid bursarId, CancellationToken cancellationToken)
        {
            var bursar = await _bursarRepository.GetBursarByIdAsync(bursarId);
            if (bursar == null)
            {
                _logger.LogError("Bursar doesn't exist");
                return new BaseResponse<BursarDto>
                {
                    Message = "Bursar doesn't exist",
                    Status = false
                };
            }

            _logger.LogInformation("Bursar fetched successfully");
            return new BaseResponse<BursarDto>
            {
                Message = "Bursar fetched successfully",
                Status = true,
                Data = new BursarDto
                {
                    Id = bursar.Id,
                    FirstName = bursar.FirstName,
                    LastName = bursar.LastName,
                    FullName = bursar.FullName(),
                    Address = bursar.Address,
                    DateOfBirth = bursar.DateOfBirth,
                    Email = bursar.User.Email,
                    PhoneNumber = bursar.PhoneNumber,
                    YearsOfExperience = bursar.YearsOfExperience,
                    DateCreated = bursar.DateCreated,

                }
            };
        }

        public async Task<BaseResponse<IReadOnlyList<BursarDto>>> GetBursarAsync(CancellationToken cancellationToken)
        {
            var bursar = await _bursarRepository.GetAllBursarAndTheirDepartment();
            if (!bursar.Any())
            {
                _logger.LogError("No data found");
                return new BaseResponse<IReadOnlyList<BursarDto>>
                {
                    Message = "No data found",
                    Status = false
                };
            }

            return new BaseResponse<IReadOnlyList<BursarDto>>
            {
                Message = "Date fetched seuccessfully",
                Status = true,
                Data = bursar.Select(r => new BursarDto
                {
                    Id = r.Id,
                    FullName = r.FullName(),
                    YearsOfExperience = r.YearsOfExperience,


                }).ToList()
            };
        }
        public async Task<BaseResponse<bool>> DeleteAsync(Guid bursarId)
        {

            var getBursar = await _bursarRepository.Get<Bursar>(b => b.Id == bursarId);
            if (getBursar == null)
            {
                _logger.LogError("Bursar coudn't be found");
                return new BaseResponse<bool>
                {
                    Message = "Bursar coudn't be found",
                    Status = false
                };
            }
            _bursarRepository.Delete<Bursar>(getBursar);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Bursar Deleted Successfully");
            return new BaseResponse<bool>
            {
                Message = "Bursar Deleted Successfully",
                Status = true
            };
        }

    }
}