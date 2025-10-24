using System.Collections.Generic;
using System.Linq;
using PhamTrungKienDAL;
using PhamTrungKienModels;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PhamTrungKienBLL
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService()
        {
            _customerRepository = DatabaseService.Instance.UnitOfWork.Customers;
        }

        public IEnumerable<Customer> GetAll() => _customerRepository.GetActiveCustomers();

        public Customer? GetById(int id) => _customerRepository.GetById(id);

        public Customer? GetByEmail(string email) => _customerRepository.GetByEmail(email);

        public bool EmailExists(string email) => _customerRepository.EmailExists(email);

        public async Task<bool> AddAsync(Customer customer)
        {
            try
            {
                if (!ValidateCustomer(customer))
                {
                    // Log validation failure details
                    System.Diagnostics.Debug.WriteLine($"Customer validation failed: Email={customer.EmailAddress}, Phone={customer.Telephone}");
                    return false;
                }

                // Check if email already exists (consider all statuses due to unique index)
                if (_customerRepository.EmailExists(customer.EmailAddress))
                {
                    System.Diagnostics.Debug.WriteLine($"Email already exists: {customer.EmailAddress}");
                    return false;
                }

                customer.CustomerStatus = 1;
                _customerRepository.Add(customer);
                await _customerRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            try
            {
                if (!ValidateCustomer(customer))
                {
                    System.Diagnostics.Debug.WriteLine($"Customer validation failed: Email={customer.EmailAddress}, Phone={customer.Telephone}");
                    return false;
                }

                // Check if email already exists for another customer (consider all records)
                if (_customerRepository.EmailExistsForOther(customer.EmailAddress, customer.CustomerID))
                {
                    System.Diagnostics.Debug.WriteLine($"Email already exists for another customer: {customer.EmailAddress}");
                    return false;
                }

                var existingCustomer = _customerRepository.GetById(customer.CustomerID);
                if (existingCustomer == null) 
                {
                    System.Diagnostics.Debug.WriteLine($"Customer not found: ID={customer.CustomerID}");
                    return false;
                }

                existingCustomer.CustomerFullName = customer.CustomerFullName;
                existingCustomer.EmailAddress = customer.EmailAddress;
                existingCustomer.Telephone = customer.Telephone;
                existingCustomer.CustomerBirthday = customer.CustomerBirthday;
                existingCustomer.Password = customer.Password;

                _customerRepository.Update(existingCustomer);
                await _customerRepository.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating customer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var existingCustomer = _customerRepository.GetById(id);
                if (existingCustomer == null) return false;

                // Check if customer has any bookings
                var bookingService = new BookingService();
                var customerBookings = bookingService.GetBookingsByCustomer(id);
                if (customerBookings.Any())
                {
                    // Soft delete - set status to deleted
                    existingCustomer.CustomerStatus = 2;
                    _customerRepository.Update(existingCustomer);
                }
                else
                {
                    // Hard delete if no bookings
                    _customerRepository.Remove(existingCustomer);
                }
                
                await _customerRepository.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TestConnection()
        {
            try
            {
                var testCustomer = _customerRepository.GetAll().FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection test failed: {ex.Message}");
                return false;
            }
        }

        public IEnumerable<Customer> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAll();

            return _customerRepository.Find(c => 
                (c.CustomerFullName != null && c.CustomerFullName.Contains(searchTerm)) ||
                (c.EmailAddress != null && c.EmailAddress.Contains(searchTerm)) ||
                (c.Telephone != null && c.Telephone.Contains(searchTerm))
            );
        }

        private bool ValidateCustomer(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.EmailAddress))
                return false;

            if (!IsValidEmail(customer.EmailAddress))
                return false;

            if (!string.IsNullOrWhiteSpace(customer.Telephone) && !IsValidPhoneNumber(customer.Telephone))
                return false;

            if (customer.CustomerBirthday.HasValue && customer.CustomerBirthday.Value > DateTime.Now)
                return false;

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                var regex = new Regex(@"^[0-9+\-\s()]+$");
                return regex.IsMatch(phoneNumber) && phoneNumber.Length >= 10;
            }
            catch
            {
                return false;
            }
        }
    }
}