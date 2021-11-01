using System.Collections.Generic;

namespace MeetingRooms.ViewModels
{
    public class DisplayViewModel
    {
        public int MeetingRoomId { get; set; }
        public string MeetingRoomName { get; set; }
        public IEnumerable<MeetingViewModel> Meetings { get; set; }
    }
}
