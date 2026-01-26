using Microsoft.AspNetCore.Identity;
using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Role;
using MITCRMS.Models.DTOs.Users;
using MITCRMS.Models.Entities;

namespace MITCRMS.Implementation.Services
{
    public class UserService : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        private readonly ITutorRepository _tutorRepository;
        private readonly IHodRepository _hodRepository;

        public UserService(IUserRepository userRepository,
            UserManager<User> userManager,
            ILogger<UserService> logger,
            ITutorRepository tutorRepository,
            IHodRepository hodRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
            _hodRepository = hodRepository;
            _tutorRepository = tutorRepository;
        }

        public async Task<BaseResponse<UserDto>> GetUserProfileByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserProfile(userId);
            if (user == null)
            {
                return new BaseResponse<UserDto>
                {
                    Message = "User not found",
                    Status = false
                };
            }
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            if (role == "Tutor")
            {
                return new BaseResponse<UserDto>
                {
                    Message = "Tutor profile fetched",
                    Status = true,
                    Data = new UserDto
                    {

                        Id = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        Tutor = new Models.DTOs.Tutor.TutorDto
                        {
                            Id = user.Tutor.Id,
                            FirstName = user.Tutor.FirstName,
                            FullName = $"{user.Tutor.FirstName} {user.Tutor.LastName}",
                            PhoneNumber = user.Tutor.PhoneNumber,
                            DateOfBirth = user.Tutor.DateOfBirth,
                            Address = user.Tutor.Address,
                        }
                    }
                };
            }


            else if (role == "Admin")
            {
                return new BaseResponse<UserDto>
                {
                    Message = "Admin profile fetched",
                    Status = true,

                    Data = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        Admin = new Models.DTOs.Admin.AdminDto
                        {
                            FirstName = user.Admin.FirstName,
                            FullName = $"{user.Admin.FirstName} {user.Admin.LastName}",
                            PhoneNumber = user.Admin.PhoneNumber,
                            DateOfBirth = user.Admin.DateOfBirth,
                            Address = user.Admin.Address,
                        }



                    }
                };
            }

            else if (role == "Bursar")
            {
                return new BaseResponse<UserDto>
                {
                    Message = "Bursar profile fetched",
                    Status = true,

                    Data = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        Bursar = new Models.DTOs.Bursar.BursarDto
                        {
                            FirstName = user.Bursar.FirstName,
                            FullName = $"{user.Bursar.FirstName} {user.Bursar.LastName}",
                            PhoneNumber = user.Bursar.PhoneNumber,
                            DateOfBirth = user.Bursar.DateOfBirth,
                            Address = user.Bursar.Address,
                        }



                    }
                };
            }

            else if (role == "Hod")
            {
                return new BaseResponse<UserDto>
                {
                    Message = "Hod profile fetched",
                    Status = true,

                    Data = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        Hod = new Models.DTOs.Hod.HodDto
                        {
                            FirstName = user.Hod.FirstName,
                            FullName = $"{user.Hod.FirstName} {user.Hod.LastName}",
                            PhoneNumber = user.Hod.PhoneNumber,
                            YearsOfExperience = user.Hod.YearsOfExperience,
                            DateOfBirth = user.Hod.DateOfBirth,
                            Address = user.Hod.Address,
                        }



                    }
                };
            }
            return new BaseResponse<UserDto>
            {
                Message = "SuperAdmin profile fetched",
                Status = true,
                Data = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = role.Select(r => new RoleDto { Name = role }).ToList(),
                   SuperAdmin = new SuperAdminDto
                    {
                        FullName = $"{user.Admin.FirstName} {user.Admin.LastName}"
                    }

                }
               
            };

        }

        public async Task<BaseResponse<LoginResponseModel>> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                _logger.LogError("Invalid User");
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Invalid User",
                    Status = false
                };
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                _logger.LogError("Invalid User");
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Invalid User",
                    Status = false
                };
            }
            var roles = await _userManager.GetRolesAsync(user);

            var role = roles.FirstOrDefault() ?? string.Empty;

            if (role == "Tutor")
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Login successful",
                    Status = true,
                    Data = new LoginResponseModel
                    {

                        UserId = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        FirstName = user.Tutor != null ? $"{user.Tutor.FirstName}" : string.Empty,
                        FullName = user.SuperAdmin != null ? $"{user.Tutor?.FullName()}" : string.Empty,

                    }
                };
            }
            if (role == "Hod")
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Login successful",
                    Status = true,

                    Data = new LoginResponseModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        FirstName = user.Hod != null ? $"{user.Hod.FirstName}" : string.Empty,
                        FullName = user.SuperAdmin != null ? $"{user.Hod?.FullName()}" : string.Empty,



                    }
                };
            }
            if (role == "Bursar")
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Login successful",
                    Status = true,

                    Data = new LoginResponseModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        FirstName = user.Bursar != null ? $"{user.Bursar.FirstName}" : string.Empty,
                        FullName = user.SuperAdmin != null ? $"{user.Bursar?.FullName()}" : string.Empty,



                    }
                };
            }
            if (role == "Admin")
            {
                return new BaseResponse<LoginResponseModel>
                {
                    Message = "Login successful",
                    Status = true,

                    Data = new LoginResponseModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = roles.Select(r => new RoleDto { Name = r }).ToList(),
                        FirstName = user.Admin != null ? $"{user.Admin.FirstName}" : string.Empty,
                        FullName = user.SuperAdmin != null ? $"{user.Admin?.FullName()}" : string.Empty,



                    }
                };
            }
            return new BaseResponse<LoginResponseModel>
            {
                Message = "Login successful",
                Status = true,
                Data = new LoginResponseModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = role.Select(r => new RoleDto { Name = role }).ToList(),
                    FirstName = user.SuperAdmin != null ? $"{user.Admin.FirstName}" : string.Empty,
                    FullName = user.SuperAdmin != null ? $"{user.Admin.FullName()}" : string.Empty,

                }
            };
        }
    }
}