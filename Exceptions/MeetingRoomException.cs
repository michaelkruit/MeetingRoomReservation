using System;

namespace MeetingRooms.Exceptions
{
    public class MeetingRoomException : Exception
    {
        public MeetingRoomException()
        {
        }

        public MeetingRoomException(string message) : base(message)
        {
        }
    }
}
