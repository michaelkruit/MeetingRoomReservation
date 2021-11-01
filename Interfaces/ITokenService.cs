using MeetingRooms.Data.Entities;

namespace MeetingRooms.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Build token
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        string BuildToken(Company company);
        
        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool ValidateToken(string token);
    }
}
