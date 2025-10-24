using PhamTrungKienModels;

namespace PhamTrungKienDAL
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Customer? GetByEmail(string email);
        IEnumerable<Customer> GetActiveCustomers();
        Task<Customer?> GetByEmailAsync(string email);
        // Duplicate checks should consider all rows regardless of status due to DB unique index
        Customer? GetByEmailAny(string email);
        bool EmailExists(string email);
        bool EmailExistsForOther(string email, int excludeCustomerId);
    }
}

