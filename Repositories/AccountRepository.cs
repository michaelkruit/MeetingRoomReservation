using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
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

        public Company GetCompany(string token)
            => _memoryCache.Get<Company>(token) ?? throw new Exception("No company found in cache");

        public async Task<string> Login(LoginViewModel loginViewModel)
        {
            var company = await _applicationDbContext.Companies.SingleOrDefaultAsync(x => x.Name.ToLower() == loginViewModel.CompanyName.ToLower());
            if (company == null)
            {
                throw new Exception("Company does not exist");
                // return error that company doesn't exist
            }

            using var hmac = new HMACSHA512(company.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginViewModel.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != company.PasswordHash[i])
                {
                    throw new Exception("Password is not correct");
                    // Password
                }
            }

            var token = _tokenService.BuildToken(company);

            _memoryCache.Set(token, company);

            return token;
        }

        public void Logout(string token) => _memoryCache.Remove(token);

        public async Task<bool> Register(RegisterViewModel registerViewModel)
        {
            if (await CompanyExist(registerViewModel.CompanyName))
            {
                return false;
                throw new Exception("Company already exists");
                // return error that company already exists
            }

            using var hmac = new HMACSHA512();

            var company = new Company()
            {
                Name = registerViewModel.CompanyName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerViewModel.Password)),
                PasswordSalt = hmac.Key
            };

            await _applicationDbContext.AddAsync(company);
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        private async Task<bool> CompanyExist(string companyName) =>
            await _applicationDbContext.Companies.AnyAsync(x => x.Name.ToLower() == companyName.ToLower());
    }
}
