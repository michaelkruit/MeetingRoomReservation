using MeetingRooms.ViewModels;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IAccountRepository
    {
        Task<string> Login(LoginViewModel loginViewModel);
        Task<string> Register(RegisterViewModel registerViewModel);
    }
}
