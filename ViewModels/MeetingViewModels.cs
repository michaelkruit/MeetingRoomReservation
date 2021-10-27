using System;
using System.Collections;
using System.Collections.Generic;

namespace MeetingRooms.ViewModels
{
    public abstract class MeetingBaseViewModel
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string AttendingCompany { get; set; }
        public int MeetingRoomId { get; set; }
        public string[] Attendees { get; set; }
    }

    public class MeetingCreateViewModel : MeetingBaseViewModel { }
    
    public class MeetingUpdateViewModel : MeetingBaseViewModel 
    {
        public int Id { get; set; }
    }

    public class MeetingViewModel : MeetingUpdateViewModel 
    {
        public string StartShortDateTimeString => $"{StartDateTime.ToShortDateString()} - {StartDateTime.ToShortTimeString()}";
        public string EndShortDateTimeString => $"{EndDateTime.ToShortDateString()} - {EndDateTime.ToShortTimeString()}";
        public MeetingRoomViewModel MeetingRoom { get; set; }
    }
}
