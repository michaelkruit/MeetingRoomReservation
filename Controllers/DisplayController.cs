using MeetingRooms.Data.Entities;
using MeetingRooms.Exceptions;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var meetingRooms = await _meetingRoomRepository.GetList(GetToken());
            var viewModel = meetingRooms.Select(x => new MeetingRoomViewModel()
            {
                CompanyId = x.CompanyId,
                Id = x.Id,
                Name = x.Name
            });

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Meetings(int meetingRoomId)
        {
            MeetingRoom meetingRoom;
            try
            {
                // Get meeting room
                meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }

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
