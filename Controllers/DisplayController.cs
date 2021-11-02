using MeetingRooms.Data.Entities;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    public class DisplayController : Controller
    {
        private readonly IMeetingRoomRepository _meetingRoomRepository;
        private readonly IMeetingRepository _meetingRepository;

        public DisplayController(IMeetingRoomRepository meetingRoomRepository, IMeetingRepository meetingRepository)
        {
            _meetingRoomRepository = meetingRoomRepository;
            _meetingRepository = meetingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Meetings(int meetingRoomId = 1)
        {
            // Get meeting room
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);

            // Get meetings
            var meetings = await _meetingRepository.GetMeetingRoomList(GetToken(), meetingRoomId);

            var viewModel = new DisplayViewModel()
            {
                MeetingRoomId = meetingRoomId,
                MeetingRoomName = meetingRoom.Name,
                Meetings = meetings.Select(x => MapMeetingsToViewModel(x))
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetMeetings(int meetingRoomId)
        {
            var meetings = await _meetingRepository.GetMeetingRoomList(GetToken(), meetingRoomId);
            var viewModel = meetings.Select(x => MapMeetingsToViewModel(x));

            return new JsonResult(viewModel);
        }

        private MeetingViewModel MapMeetingsToViewModel(Meeting meeting)
            => new MeetingViewModel()
            {
                AttendingCompany = meeting.AttendingCompany,
                EndDateTime = meeting.EndDatetime,
                Id = meeting.Id,
                StartDateTime = meeting.StartDatetime,
                Attendees = string.IsNullOrEmpty(meeting.Attendees?.Names) == false ? meeting.Attendees?.Names.Split(",") : null
            };

        private string GetToken() => HttpContext.Session.GetString("Token");
    }
}
