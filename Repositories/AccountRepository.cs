using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
using MeetingRooms.Interfaces;
using MeetingRooms.Models;
using MeetingRooms.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MeetingRooms.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly HMACSHA512 _hmac;

        public AccountRepository(ApplicationDbContext applicationDbContext, HMACSHA512 hmac)
        {
            _applicationDbContext = applicationDbContext;
            _hmac = hmac;
        }

        public async Task<string> Login(LoginViewModel loginViewModel)
        {
            var company = await _applicationDbContext.Companies.SingleOrDefaultAsync(x => x.Name.ToLower() == loginViewModel.CompanyName.ToLower());
            if(company == null)
            {
                throw new Exception("Company does not exist");
                // return error that company doesn't exist
            }

            var computedHash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(loginViewModel.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != company.PasswordHash[i])
                {
                    throw new Exception("Password is not correct");
                    // Password
                }
            }

            var companyUser = new CompanyUserModel()
            {
                Id = company.Id,
                Name = company.Name,
                Token = string.Empty // TODO: Generate JWT
            };

            throw new NotImplementedException();
        }

        public async Task<string> Register(RegisterViewModel registerViewModel)
        {
            if (await CompanyExist(registerViewModel.CompanyName) == false)
            {
                throw new Exception("Company already exists");
                // return error that company already exists
            }

            var company = new Company()
            {
                Name = registerViewModel.CompanyName,
                PasswordHash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(registerViewModel.Password))
            };

            await _applicationDbContext.AddAsync(company);
            await _applicationDbContext.SaveChangesAsync();

            var companyUser = new CompanyUserModel()
            {
                Id = company.Id,
                Name = company.Name,
                Token = string.Empty // TODO: Generate JWT
            };

            throw new NotImplementedException();
        }

        private async Task<bool> CompanyExist(string companyName) =>
            await _applicationDbContext.Companies.AnyAsync(x => x.Name.ToLower() == companyName.ToLower());
    }
}
