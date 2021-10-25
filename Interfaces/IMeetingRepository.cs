using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IMeetingRepository
    {
        Task<IEnumerable<Meeting>> GetCompanyList(string token);
        Task<IEnumerable<Meeting>> GetMeetingRoomList(string token, int meetingRoomId);
        Task<Meeting> GetSingle(string token, int id);
        Task<Meeting> Create(string token, MeetingCreateViewModel createModel);
        Task<Meeting> Update(string token, MeetingUpdateViewModel updateModel);
        Task<bool> Delete(string token, int id);
    }
}
