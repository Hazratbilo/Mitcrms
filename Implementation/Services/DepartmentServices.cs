using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MITCRMS.Contract.Services;
using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Department;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Entities;

namespace MITCRMS.Implementation.Services
{
    public class DepartmentServices : IDepartmentServices
    {

        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IRoleRepository _roleRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DepartmentServices> _logger;
        public DepartmentServices(IUserRepository userRepository,
            UserManager<User> userManager,
            IIdentityService identityService,
            IRoleRepository roleRepository,
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            ILogger<DepartmentServices> logger)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _identityService = identityService;
            _roleRepository = roleRepository;
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<bool>> AddDepartment(CreateDepartmentRequestModel request)
        {
            if (request == null)
            {
                return new BaseResponse<bool>
                {
                    Message = "Fields cannot be empty",
                    Status = false,
                };
            }
            var departmentExists = await _departmentRepository.ExistsByName(request.DepartmentName);
            if (departmentExists)
            {
                _logger.LogError("Department already exists");
                return new BaseResponse<bool>
                {
                    Message = $"Department with name {request.DepartmentName} already exists",
                    Status = false
                };
            }

            var newDepartment = new Department
            {
                DepartmentName = request.DepartmentName,
                DepartmentCode = request.DepartmentCode,
                DateCreated = DateTime.UtcNow
            };
            var createDepartment = await _departmentRepository.Add<Department>(newDepartment);
            await _unitOfWork.SaveChangesAsync();
            if (createDepartment == null)
            {
                _logger.LogError("Create Department Failed");
                return new BaseResponse<bool>
                {
                    Message = "Create Department Failed",
                    Status = false
                };
            }
            _logger.LogInformation("Create Department Success");
            return new BaseResponse<bool>
            {
                Message = "Create Department Success",
                Status = true,

            };
        }

        public async Task<BaseResponse<bool>> DeleteDepartment(Guid Id)
        {
            var getDepartment = await _departmentRepository.GetDepartmentById(Id);
            if (getDepartment == null)
            {
                _logger.LogError($"Department with id {Id} not found");
                return new BaseResponse<bool>
                {
                    Message = $"Department with id {Id} not found",
                    Status = false
                };
            }

            var deleteDepartment = await _departmentRepository.DeleteDepartment(Id);
            if (deleteDepartment)
            {
                _logger.LogInformation("Delete Department Success");
                return new BaseResponse<bool>
                {
                    Message = "Delete Department Success",
                    Status = true
                };
            }
            _logger.LogError("Delete Department Failed");
            return new BaseResponse<bool>
            {
                Message = "Delete Department Failed",
                Status = false
            };
        }

        public async Task<BaseResponse<List<DepartmentDto>>> GetAllDepartmentsAndAdmin()
        {
            var getAllDepartmentsAndAdmin = await _departmentRepository.GetAllDepartmentsAndAdmin();
            if (!getAllDepartmentsAndAdmin.Any())
            {
                _logger.LogError($"No department found");
                return new BaseResponse<List<DepartmentDto>>
                {
                    Message = $"No department found",
                    Status = false,
                };
            }
            _logger.LogInformation("All departments along their Admin fetched successfully");
            return new BaseResponse<List<DepartmentDto>>
            {
                Message = "All departments along their Admin fetched successfully",
                Status = true,
                Data = getAllDepartmentsAndAdmin.Select(dpt => new DepartmentDto
                {
                    Id = dpt.Id,
                    DepartmentName = dpt.DepartmentName,
                    DepartmentCode = dpt.DepartmentCode,
                    DateCreated = dpt.DateCreated,
                    Admins = dpt.Admins.Select(a => new AdminDto()
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        PhoneNumber = a.PhoneNumber,
                        DateCreated = a.DateCreated,
                        Address = a.Address,

                    }).ToList()

                }).ToList()
            };
        }

        public async Task<BaseResponse<List<DepartmentDto>>> GetAllDepartmentsAndBursar()
        {
            var getAllDepartmentsAndBursar = await _departmentRepository.GetAllDepartmentsAndBursar();
            if (!getAllDepartmentsAndBursar.Any())
            {
                _logger.LogError($"No department found");
                return new BaseResponse<List<DepartmentDto>>
                {
                    Message = $"No department found",
                    Status = false,
                };
            }
            _logger.LogInformation("All departments along their Bursar fetched successfully");
            return new BaseResponse<List<DepartmentDto>>
            {
                Message = "All departments along their Bursar fetched successfully",
                Status = true,
                Data = getAllDepartmentsAndBursar.Select(dpt => new DepartmentDto
                {
                    Id = dpt.Id,
                    DepartmentName = dpt.DepartmentName,
                    DepartmentCode = dpt.DepartmentCode,
                    DateCreated = dpt.DateCreated,
                    Bursars = dpt.Bursars.Select(a => new BursarDto()
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        PhoneNumber = a.PhoneNumber,
                        DateCreated = a.DateCreated,
                        Address = a.Address,

                    }).ToList()

                }).ToList()
            };
        }


        public async Task<BaseResponse<List<DepartmentDto>>> GetAllDepartmentsAndHod()
        {
            var getAllDepartmentsAndHod = await _departmentRepository.GetAllDepartmentsAndHod();
            if (!getAllDepartmentsAndHod.Any())
            {
                _logger.LogError($"No department found");
                return new BaseResponse<List<DepartmentDto>>
                {
                    Message = $"No department found",
                    Status = false,
                };
            }
            _logger.LogInformation("All departments along their Hod fetched successfully");
            return new BaseResponse<List<DepartmentDto>>
            {
                Message = "All departments along their Hod fetched successfully",
                Status = true,
                Data = getAllDepartmentsAndHod.Select(dpt => new DepartmentDto
                {
                    Id = dpt.Id,
                    DepartmentName = dpt.DepartmentName,
                    DepartmentCode = dpt.DepartmentCode,
                    DateCreated = dpt.DateCreated,
                    Hods = dpt.Hods.Select(a => new HodDto()
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        PhoneNumber = a.PhoneNumber,
                        DateCreated = a.DateCreated,
                        Address = a.Address,

                    }).ToList()

                }).ToList()
            };
        }



        public async Task<BaseResponse<List<DepartmentDto>>> GetAllDepartmentsAndTutor()
        {
            var getAllDepartmentsAndTutor = await _departmentRepository.GetAllDepartmentsAndTutor();
            if (!getAllDepartmentsAndTutor.Any())
            {
                _logger.LogError($"No department found");
                return new BaseResponse<List<DepartmentDto>>
                {
                    Message = $"No department found",
                    Status = false,
                };
            }
            _logger.LogInformation("All departments along their Tutor fetched successfully");
            return new BaseResponse<List<DepartmentDto>>
            {
                Message = "All departments along their Tutor fetched successfully",
                Status = true,
                Data = getAllDepartmentsAndTutor.Select(dpt => new DepartmentDto
                {
                    Id = dpt.Id,
                    DepartmentName = dpt.DepartmentName,
                    DepartmentCode = dpt.DepartmentCode,
                    DateCreated = dpt.DateCreated,
                    Tutors = dpt.Tutors.Select(a => new TutorDto()
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        PhoneNumber = a.PhoneNumber,
                        DateCreated = a.DateCreated,
                        Address = a.Address,

                    }).ToList()

                }).ToList()
            };
        }

        public async Task<BaseResponse<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            var getAllDepartments = await _departmentRepository.GetAllDepartments();
            if (!getAllDepartments.Any())
            {
                _logger.LogError($"No department found");
                return new BaseResponse<IEnumerable<DepartmentDto>>
                {
                    Message = $"No department found",
                    Status = false,
                };
            }
            _logger.LogInformation("All departments fetched successfully");
            return new BaseResponse<IEnumerable<DepartmentDto>>
            {
                Message = "All departments fetched successfully",
                Status = true,
                Data = getAllDepartments.Select(dpt => new DepartmentDto
                {
                    Id = dpt.Id,
                    DepartmentName = dpt.DepartmentName,
                    DepartmentCode = dpt.DepartmentCode,
                    DateCreated = dpt.DateCreated,
                }).ToList()
            };
        }

        public async Task<BaseResponse<DepartmentDto>> GetDepartmentById(Guid Id)
        {
            var getDepartment = await _departmentRepository.GetDepartmentById(Id);
            if (getDepartment == null)
            {
                _logger.LogError($"Department with id {Id} not found");
                return new BaseResponse<DepartmentDto>
                {
                    Message = $"Department with id {Id} not found",
                    Status = false,
                };
            }
            _logger.LogInformation("Department fetched successfully");
            return new BaseResponse<DepartmentDto>
            {
                Message = "Department fetched successfully",
                Status = true,
                Data = new DepartmentDto
                {
                    Id = getDepartment.Id,
                    DepartmentName = getDepartment.DepartmentName,
                    DepartmentCode = getDepartment.DepartmentCode,
                    DateCreated = getDepartment.DateCreated,
                }
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetDepartmentsSelectList()
        {
            var depts = await _departmentRepository.GetAllDepartments();
            return depts.Select(dpt => new SelectListItem
            {
                Text = dpt.DepartmentName,
                Value = dpt.Id.ToString()
            }).ToList();
        }
    }
}
