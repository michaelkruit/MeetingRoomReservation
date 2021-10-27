using MeetingRooms.Data.Entities;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingController(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var meetings = await _meetingRepository.GetCompanyList(GetToken());

            var meetingsViewModel = meetings.Select(x => MapMeetingViewModel(x));

            return View(meetingsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);

            var viewModel = MapMeetingViewModel(meeting);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(int meetingRoomId)
        {
            var viewModel = new MeetingCreateViewModel()
            {
                MeetingRoomId = meetingRoomId
            };

            return View(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> Create(MeetingCreateViewModel createViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(createViewModel);
            }

            var meeting = await _meetingRepository.Create(GetToken(), createViewModel);

            return RedirectToAction(nameof(Details), new { id = meeting.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);

            // Get attendees

            var updateViewModel = new MeetingUpdateViewModel()
            {
                AttendingCompany = meeting.AttendingCompany,
                EndDateTime = meeting.EndDatetime,
                Id = meeting.Id,
                MeetingRoomId = meeting.MeetingRoomId,
                StartDateTime = meeting.StartDatetime
            };

            return View(updateViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingUpdateViewModel updateViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(updateViewModel);
            }

            await _meetingRepository.Update(GetToken(), updateViewModel);

            return RedirectToAction(nameof(Details), new { id = updateViewModel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);
            var meetingViewModel = MapMeetingViewModel(meeting);
            return View(meetingViewModel);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var deleted = await _meetingRepository.Delete(GetToken(), id);

            return deleted ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Delete), new { id });
        }

        // private helpers
        private string GetToken() => HttpContext.Session.GetString("Token");

        private MeetingViewModel MapMeetingViewModel(Meeting meeting) => new MeetingViewModel()
        {
            Id = meeting.Id,
            MeetingRoomId = meeting.MeetingRoomId,
            StartDateTime = meeting.StartDatetime,
            EndDateTime = meeting.EndDatetime,
            Attendees = string.IsNullOrEmpty(meeting.Attendees?.Names) == false ? meeting.Attendees?.Names.Split(",") : null,
            MeetingRoom = new MeetingRoomViewModel()
            {
                CompanyId = meeting.MeetingRoom.CompanyId,
                Id = meeting.MeetingRoom.Id,
                Name = meeting.MeetingRoom.Name
            }
        };
    }
}
