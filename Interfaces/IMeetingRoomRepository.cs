using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IMeetingRoomRepository
    {
        /// <summary>
        /// Get list of meetingrooms
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<MeetingRoom>> GetList(string token);
        
        /// <summary>
        /// Get one meeting room
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MeetingRoom> GetSingle(int id);
        
        /// <summary>
        /// Create new meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="createModel"></param>
        /// <returns></returns>
        Task<MeetingRoom> Create(string token, MeetingRoomCreateViewModel createModel);
        
        /// <summary>
        /// Update existing meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        Task<MeetingRoom> Update(string token, MeetingRoomUpdateViewModel updateModel);
        
        /// <summary>
        /// Delete selected meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(string token, int id);
    }
}
