using System.Linq;
using PhamTrungKienDAL;
using PhamTrungKienModels;

namespace PhamTrungKienBLL
{
    public class AuthService
    {
        private readonly DatabaseService db = DatabaseService.Instance;

        public Customer? Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            // Check admin account first
            if (email.ToLower() == AdminConfig.AdminEmail.ToLower() && password == AdminConfig.AdminPassword)
            {
                return new Customer
                {
                    CustomerID = 0, // Special ID for admin
                    CustomerFullName = "System Administrator",
                    EmailAddress = AdminConfig.AdminEmail,
                    CustomerStatus = 1
                };
            }

            // Check regular customers
            return db.Customers.FirstOrDefault(c => c.EmailAddress.ToLower() == email.ToLower()
                                                   && c.Password == password
                                                   && c.CustomerStatus == 1);
        }
    }
}

