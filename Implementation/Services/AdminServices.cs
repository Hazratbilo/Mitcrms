using Microsoft.AspNetCore.Identity;
using MITCRMS.Contract.Services;
using MITCRMS.Implementation.Repository;
using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Entities;

namespace MITCRMS.Implementation.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IUnitOfWork _unitOfWork;
        ILogger<AdminServices> _logger;
        public AdminServices(IUserRepository userRepository,
            UserManager<User> userManager,
            IIdentityService identityService,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            IAdminRepository adminRepository,
            IUnitOfWork unitOfWork,
            ILogger<AdminServices> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _identityService = identityService;
            _departmentRepository = departmentRepository;
            _roleRepository = roleRepository;
            _adminRepository = adminRepository;
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

        public async Task<BaseResponse<bool>> CreateAdminAsync(CreateAdminRequestModel request)
        {
            var adminExists = await _userRepository.Any(u => u.Email == request.Email);
            if (adminExists)
            {
                _logger.LogError("Admin with email already exist");
                return new BaseResponse<bool>
                {
                    Message = "Admin with email already exist",
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
                var AdminUser = new User
                {
                    Email = request.Email,
                    PasswordHash = _identityService.GetPasswordHash(request.PasswordHash)
                };

                var newUser = await _userManager.CreateAsync(AdminUser);
                if (newUser == null)
                {
                    _logger.LogError("User Creation unsuccessful");
                    return new BaseResponse<bool>
                    {
                        Message = "User Creation unsuccessful",
                        Status = false
                    };

                }

                var result = await _userManager.AddToRoleAsync(AdminUser, "Admin");
                if (!result.Succeeded)
                {
                    _logger.LogError("Unable to add user to roles");
                    return new BaseResponse<bool>
                    {
                        Message = "Unable to add user to roles",
                        Status = false
                    };
                }

                var userRoles = await _userManager.GetRolesAsync(AdminUser);


                if (!result.Succeeded)
                {
                    throw new Exception($"Unable to add Admin to roles");
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

                var Admin = new Admin
                {

                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    PhoneNumber = request.PhoneNumber,
                    UserId = AdminUser.Id,
                    DateCreated = DateTime.UtcNow,
                    DepartmentId = dept.Id,
                };

                var createAdmin = await _adminRepository.Add(Admin);
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);

                if (createAdmin == null)
                {
                    _logger.LogError("Admin couldn't be added");
                    return new BaseResponse<bool>
                    {
                        Message = "Admin couldn't be added",
                        Status = false,
                    };
                }

                await transaction.CommitAsync();
                _logger.LogInformation("Admin added successfully");
                return new BaseResponse<bool>
                {
                    Message = "Admin added successfully",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error  creating admin, rolling back.....");
                return new BaseResponse<bool>
                {
                    Message = "Error  creating admin, rolling back.....",
                    Status = false
                };
            }
        }

        public async Task<BaseResponse<AdminDto>> GetAdminByIdAsync(Guid adminId, CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetAdminByIdAsync(adminId);
            if (admin == null)
            {
                _logger.LogError("Admin doesn't exist");
                return new BaseResponse<AdminDto>
                {
                    Message = "Admin doesn't exist",
                    Status = false
                };
            }

            _logger.LogInformation("Admin fetched successfully");
            return new BaseResponse<AdminDto>
            {
                Message = "Admin fetched successfully",
                Status = true,
                Data = new AdminDto
                {
                    Id = admin.Id,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    FullName = admin.FullName(),
                    Address = admin.Address,
                    DateOfBirth = admin.DateOfBirth,
                    Email = admin.User.Email,
                    PhoneNumber = admin.PhoneNumber,
                    YearsOfExperience = admin.YearsOfExperience,
                    DateCreated = admin.DateCreated,

                }
            };
        }

        public async Task<BaseResponse<IReadOnlyList<AdminDto>>> GetAdminAsync(CancellationToken cancellationToken)
        {
            var admin = await _adminRepository.GetAllAdminAndTheirDepartment();
            if (!admin.Any())
            {
                _logger.LogError("No data found");
                return new BaseResponse<IReadOnlyList<AdminDto>>
                {
                    Message = "No data found",
                    Status = false
                };
            }

            return new BaseResponse<IReadOnlyList<AdminDto>>
            {
                Message = "Date fetched seuccessfully",
                Status = true,
                Data = admin.Select(r => new AdminDto
                {
                    Id = r.Id,
                    FullName = r.FullName(),
                    YearsOfExperience = r.YearsOfExperience,


                }).ToList()
            };
        }
        public async Task<BaseResponse<bool>> DeleteAsync(Guid adminId)
        {

            var getAdmin = await _adminRepository.Get<Admin>(a=> a.Id == adminId);
            if (getAdmin == null)
            {
                _logger.LogError("Admin coudn't be found");
                return new BaseResponse<bool>
                {
                    Message = "Admin coudn't be found",
                    Status = false
                };
            }
            _adminRepository.Delete<Admin>(getAdmin);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            _logger.LogInformation("Admin Deleted Successfully");
            return new BaseResponse<bool>
            {
                Message = "Admin Deleted Successfully",
                Status = true
            };
        }
    }
}