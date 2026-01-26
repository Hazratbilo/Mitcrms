using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;

namespace MITCRMS.Implementation.Repository
{
    public class DepartmentRepository : BaseRepository, IDepartmentRepository
    {
        public DepartmentRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {
        }

        //public async Task<Department> AddDepartment(Department department)
        //{
        //    await _mitcrmsContext.Set<Department>().AddAsync(department);
        //    await _mitcrmsContext.SaveChangesAsync();
        //    return department;
        //}

        public async Task<bool> DeleteDepartment(Guid id)
        {
            var department = await _mitcrmsContext.Set<Department>().FindAsync(id);
            if (department == null)
                return false;

            _mitcrmsContext.Set<Department>().Remove(department);
            await _mitcrmsContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByName(string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
                return false;

            var name = departmentName.Trim().ToLowerInvariant();
            return await _mitcrmsContext.Set<Department>()
                .AnyAsync(d => d.DepartmentName != null && d.DepartmentName.ToLower() == name);
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await _mitcrmsContext.Set<Department>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAndAdmin()
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Admins)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAndBursar()
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Bursars)
                .AsNoTracking()
                .ToListAsync();
        }



        public async Task<List<Department>> GetAllDepartmentsAndHod()
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Hod)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAndReports()
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Tutors)
                .Include(d => d.Hod)
                .Include(d => d.Reports)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Department>> GetAllDepartmentsAndTutor()
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Tutors)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentById(Guid id)
        {
            return await _mitcrmsContext.Set<Department>()
                .Include(d => d.Tutors)
                .Include(d => d.Hod)
                .Include(d => d.Admins)
                .Include(d => d.Bursars)
                .Include(d => d.Reports)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Department> UpdateDepartment(Department department)
        {
            _mitcrmsContext.Set<Department>().Update(department);
            await _mitcrmsContext.SaveChangesAsync();
            return department;
        }
    }
}