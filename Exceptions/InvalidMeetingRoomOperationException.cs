using System;

namespace MeetingRooms.Exceptions
{
    public class InvalidMeetingRoomOperationException : Exception
    {
        public InvalidMeetingRoomOperationException(string message) : base(message)
        {
        }
    }
}
