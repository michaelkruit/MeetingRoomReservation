using MeetingRooms.Data.Entities;

namespace MeetingRooms.Interfaces
{
    public interface ITokenService
    {
        string BuildToken(Company company);
        bool ValidateToken(string token);
    }
}
