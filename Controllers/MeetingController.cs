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
            // Get meetings
            var meetings = await _meetingRepository.GetCompanyList(GetToken());
            
            // Map meetings to viewmodels
            var meetingsViewModel = meetings.Select(x => MapMeetingViewModel(x));

            return View(meetingsViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Get selected meeting
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);

            // Map meeting to viewmodel
            var viewModel = MapMeetingViewModel(meeting);

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create(int meetingRoomId)
        {
            // Create new meeting with meeting and set meetingroom when available (can be 0)
            var viewModel = new MeetingCreateViewModel()
            {
                MeetingRoomId = meetingRoomId
            };

            return View(viewModel);
        }

        [HttpPut]
        public async Task<IActionResult> Create(MeetingCreateViewModel createViewModel)
        {
            // Check if model is filled correct
            if (!ModelState.IsValid)
            {
                return View(createViewModel);
            }

            // Create new meeting
            var meeting = await _meetingRepository.Create(GetToken(), createViewModel);

            // Redirect to meeting details
            return RedirectToAction(nameof(Details), new { id = meeting.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Get selected meeting
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);

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
            // Check if model is filled in correct
            if (!ModelState.IsValid)
            {
                return View(updateViewModel);
            }

            // Update meeting
            await _meetingRepository.Update(GetToken(), updateViewModel);

            // Redirect to details
            return RedirectToAction(nameof(Details), new { id = updateViewModel.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Get selected meeting
            var meeting = await _meetingRepository.GetSingle(GetToken(), id);

            // Map to viewmodel
            var meetingViewModel = MapMeetingViewModel(meeting);

            return View(meetingViewModel);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult>DeleteConfirmed(int id)
        {
            // Delete meeting
            var deleted = await _meetingRepository.Delete(GetToken(), id);
            // Redirect to index is success and redirect to Delete is failed
            return deleted ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Delete), new { id });
        }

        // private helpers
        /// <summary>
        /// Get token from session
        /// </summary>
        /// <returns></returns>
        private string GetToken() => HttpContext.Session.GetString("Token");

        /// <summary>
        /// Map meeting to viewmodel
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
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
