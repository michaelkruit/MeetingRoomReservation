using System.ComponentModel.DataAnnotations;

namespace MeetingRooms.ViewModels
{
    public abstract class MeetingRoomBaseViewModel
    {
        [Required]
        public string Name { get; set; }
        public int CompanyId { get; set; }
    }

    public class MeetingRoomCreateViewModel : MeetingRoomBaseViewModel { }
    
    public class MeetingRoomUpdateViewModel : MeetingRoomBaseViewModel 
    {
        public int Id { get; set; }
    }

    public class MeetingRoomViewModel : MeetingRoomUpdateViewModel { }
}
