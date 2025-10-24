using Microsoft.EntityFrameworkCore;
using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(HotelDbContext context) : base(context)
        {
        }

        public Customer? GetByEmail(string email)
        {
            return _dbSet.FirstOrDefault(c => c.EmailAddress == email && c.CustomerStatus == 1);
        }

        public IEnumerable<Customer> GetActiveCustomers()
        {
            return _dbSet.Where(c => c.CustomerStatus == 1).ToList();
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.EmailAddress == email && c.CustomerStatus == 1);
        }

        public Customer? GetByEmailAny(string email)
        {
            return _dbSet.FirstOrDefault(c => c.EmailAddress == email);
        }

        public bool EmailExists(string email)
        {
            return _dbSet.Any(c => c.EmailAddress == email);
        }

        public bool EmailExistsForOther(string email, int excludeCustomerId)
        {
            return _dbSet.Any(c => c.EmailAddress == email && c.CustomerID != excludeCustomerId);
        }
    }
}

