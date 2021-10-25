using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> Login(LoginViewModel loginViewModel);
        Task<bool> Register(RegisterViewModel registerViewModel);
        void Logout(string token);
        Company GetCompany(string token);
    }
}
