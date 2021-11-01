using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Log company in
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        Task<string> Login(LoginViewModel loginViewModel);

        /// <summary>
        /// Register new company
        /// </summary>
        /// <param name="registerViewModel"></param>
        /// <returns></returns>
        Task<bool> Register(RegisterViewModel registerViewModel);
        
        /// <summary>
        /// Log user out
        /// </summary>
        /// <param name="token"></param>
        void Logout(string token);

        /// <summary>
        /// Get loged in company
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Company GetCompany(string token);
    }
}
