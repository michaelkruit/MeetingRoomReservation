using System;

namespace MeetingRooms.Exceptions
{
    public class AccountException : Exception
    {
        public AccountException(string message) : base(message)
        {
        }
    }
}
