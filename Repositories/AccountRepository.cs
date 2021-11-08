using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
using MeetingRooms.Exceptions;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRooms.Repositories
{
    [AllowAnonymous]
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _memoryCache;

        public AccountRepository(ApplicationDbContext applicationDbContext, ITokenService tokenService, IMemoryCache memoryCache)
        {
            _applicationDbContext = applicationDbContext;
            _tokenService = tokenService;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get company registered with current token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Company GetCompany(string token)
            => _memoryCache.Get<Company>(token) ?? throw new Exception("No company found in cache");

        /// <summary>
        /// Log in user
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        public async Task<string> Login(LoginViewModel loginViewModel)
        {
            // Get company
            var company = await _applicationDbContext.Companies.SingleOrDefaultAsync(x => x.Name.ToLower() == loginViewModel.CompanyName.ToLower()) 
                ?? throw new AccountException("Company does not exist");

            // Initialze new HAMCSHA512 with password salt
            using var hmac = new HMACSHA512(company.PasswordSalt);

            // Compute password has
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginViewModel.Password));
            
            // Compare computed hash with stored hash 
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != company.PasswordHash[i])
                {
                    throw new AccountException("Password is not correct");
                }
            }

            // Build token 
            var token = _tokenService.BuildToken(company);

            // Store company with token
            _memoryCache.Set(token, company);

            // Return token
            return token;
        }

        /// <summary>
        /// Log user out
        /// Remove token from header
        /// </summary>
        /// <param name="token"></param>
        public void Logout(string token) => _memoryCache.Remove(token);

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        public async Task<bool> Register(RegisterViewModel registerViewModel)
        {
            // Check if company name not already exist
            if (await CompanyExist(registerViewModel.CompanyName))
            {
                throw new AccountException($"Company '{registerViewModel.CompanyName}' already exist");
            }

            // Initialize new HMACSHA512
            using var hmac = new HMACSHA512();

            // Create new company entity
            var company = new Company()
            {
                Name = registerViewModel.CompanyName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerViewModel.Password)),
                PasswordSalt = hmac.Key
            };

            // Add entity to DB
            await _applicationDbContext.AddAsync(company);
            // Save changes and return is we saved more then 0 records
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Helper method to check if the company already exist
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        private async Task<bool> CompanyExist(string companyName) =>
            await _applicationDbContext.Companies.AnyAsync(x => x.Name.ToLower() == companyName.ToLower());
    }
}
