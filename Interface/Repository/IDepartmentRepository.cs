using MITCRMS.Models.Entities;

namespace MITCRMS.Interface.Repository
{
    public interface IDepartmentRepository:IBaseRepository
    {
        //public Task<Department> AddDepartment(Department department);
        public Task<bool> DeleteDepartment(Guid id);
        Task<List<Department>> GetAllDepartments();
        public Task<Department> GetDepartmentById(Guid id);
        Task<bool> ExistsByName(string departmentName);
        Task<List<Department>> GetAllDepartmentsAndReports();
        Task<Department> UpdateDepartment(Department department);
        Task<List<Department>> GetAllDepartmentsAndHod();
        Task<List<Department>> GetAllDepartmentsAndTutor();
        Task<List<Department>> GetAllDepartmentsAndBursar();
        Task<List<Department>> GetAllDepartmentsAndAdmin();
    }
}
