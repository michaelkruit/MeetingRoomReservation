using MeetingRooms.Data.Entities;
using MeetingRooms.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRooms.Interfaces
{
    public interface IMeetingRoomRepository
    {
        Task<IEnumerable<MeetingRoom>> GetList(string token);
        Task<MeetingRoom> GetSingle(int id);
        Task<MeetingRoom> Create(string token, MeetingRoomCreateViewModel createModel);
        Task<MeetingRoom> Update(string token, MeetingRoomUpdateViewModel updateModel);
        Task<bool> Delete(string token, int id);
    }
}
