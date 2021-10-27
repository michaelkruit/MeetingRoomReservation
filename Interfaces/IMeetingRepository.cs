using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IMeetingRepository
    {
        /// <summary>
        /// Get all meetings belonging to company where start datetime => today
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Meeting>> GetCompanyList(string token);

        /// <summary>
        /// Get all meetings of selected meetingroom where start datetime => today
        /// </summary>
        /// <param name="token"></param>
        /// <param name="meetingRoomId"></param>
        /// <returns></returns>
        Task<IEnumerable<Meeting>> GetMeetingRoomList(string token, int meetingRoomId);
        
        /// <summary>
        /// Get single meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Meeting> GetSingle(string token, int id);

        /// <summary>
        /// Create new meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="createModel"></param>
        /// <returns></returns>
        Task<Meeting> Create(string token, MeetingCreateViewModel createModel);

        /// <summary>
        /// Update existing meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        Task<Meeting> Update(string token, MeetingUpdateViewModel updateModel);

        /// <summary>
        /// Delete selected meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(string token, int id);
    }
}
