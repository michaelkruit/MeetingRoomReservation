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

        public async Task<IActionResult> ShowMeetings(int meetingRoomId)
        {
            // Get meeting room
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);

            // Get meetings
            var meetings = await _meetingRepository.GetMeetingRoomList(GetToken(), meetingRoomId);

            var viewModel = new DisplayViewModel()
            {
                MeetingRoomId = meetingRoomId,
                MeetingRoomName = meetingRoom.Name,
                Meetings = meetings.Select(x => new MeetingViewModel()
                {
                    Id = x.Id,
                    AttendingCompany = x.AttendingCompany,
                    StartDateTime = x.StartDatetime,
                    EndDateTime = x.EndDatetime
                })
            };

            return View(viewModel);
        }

        public async Task<JsonResult> GetMeetings(int meetingRoomId)
        {
            var meetings = await _meetingRepository.GetMeetingRoomList(GetToken(), meetingRoomId);
            return new JsonResult(new { meetings });
        }

        private string GetToken() => HttpContext.Session.GetString("Token");
    }
}
