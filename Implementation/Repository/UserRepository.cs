using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;
using System.Linq.Expressions;

namespace MITCRMS.Implementation.Repository
{
    public class UserRepository : BaseRepository, IUserRepository

    {
        public UserRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {
        }

        public async Task<bool> Any(Expression<Func<User, bool>> expression)
        {
            return await _mitcrmsContext.Set<User>()
             .AnyAsync(expression);
        }

        public async Task<IReadOnlyList<User>> GetByRole(Expression<Func<User, bool>> expression)
        {
            return await _mitcrmsContext.Set<User>()
                .Where(expression)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetUserAndRoles(Guid userId)
        {
            return await _mitcrmsContext.Set<User>()
                .Where(u => u.Id == userId)
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _mitcrmsContext.Set<User>()
                .Include(u => u.UserRoles)
                .ThenInclude(u => u.Role)
                .Include(p => p.Admin)
                .Include(d => d.Hod)
                .Include(p => p.Tutor)
                .ThenInclude(p => p.Department)
                .SingleOrDefaultAsync(u => u.Email == email);

        }

        public async Task<User> GetUserProfile(Guid userId)
        {
            return await _mitcrmsContext.Set<User>()
                  .Where(u => u.Id == userId)
                  .Include(a => a.Admin)
                  .Include(p => p.Tutor)
                  .ThenInclude(p => p.Department)
                  .Include(d => d.Hod)
                  .Include(u => u.UserRoles)
                  .ThenInclude(ur => ur.Role)
                   .AsSplitQuery()
                  .AsNoTracking()
                  .SingleOrDefaultAsync();
        }
    }
}
