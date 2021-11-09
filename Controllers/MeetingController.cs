using MeetingRooms.Data.Entities;
using MeetingRooms.Exceptions;
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
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IMeetingRoomRepository _meetingRoomRepository;

        public MeetingController(IMeetingRepository meetingRepository, IMeetingRoomRepository meetingRoomRepository)
        {
            _meetingRepository = meetingRepository;
            _meetingRoomRepository = meetingRoomRepository;
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
            try
            {
                // Get selected meeting
                var meeting = await _meetingRepository.GetSingle(GetToken(), id);

                // Map meeting to viewmodel
                var viewModel = MapMeetingViewModel(meeting);

                return View(viewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int meetingRoomId)
        {
            // Create new meeting with meeting and set meetingroom when available (can be 0)
            var viewModel = new MeetingCreateViewModel()
            {
                MeetingRoomId = meetingRoomId,
                MeetingRooms = await GetMeetingRooms()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeetingCreateViewModel createViewModel)
        {
            // Check if model is filled correct
            if (!ModelState.IsValid)
            {
                createViewModel.MeetingRooms = await GetMeetingRooms();
                return View(createViewModel);
            }

            try
            {
                // Create new meeting
                var meeting = await _meetingRepository.Create(GetToken(), createViewModel);
                // Redirect to meeting details
                return RedirectToAction(nameof(Details), new { id = meeting.Id });
            }
            catch (MeetingRoomException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(createViewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                // Get selected meeting
                var meeting = await _meetingRepository.GetSingle(GetToken(), id);

                var updateViewModel = MapMeetingViewModel(meeting);
                updateViewModel.MeetingRooms = await GetMeetingRooms();

                return View(updateViewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingUpdateViewModel updateViewModel)
        {
            // Check if model is filled in correct
            if (!ModelState.IsValid)
            {
                updateViewModel.MeetingRooms = await GetMeetingRooms();
                return View(updateViewModel);
            }

            try
            {
                // Update meeting
                await _meetingRepository.Update(GetToken(), updateViewModel);

                // Redirect to details
                return RedirectToAction(nameof(Details), new { id = updateViewModel.Id });
            }
            catch (MeetingRoomException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(updateViewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Get selected meeting
                var meeting = await _meetingRepository.GetSingle(GetToken(), id);

                // Map to viewmodel
                var meetingViewModel = MapMeetingViewModel(meeting);

                return View(meetingViewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Delete meeting
                var deleted = await _meetingRepository.Delete(GetToken(), id);
                // Redirect to index is success and redirect to Delete is failed
                return deleted ? RedirectToAction(nameof(Index)) : RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception e) when (e.GetType() == typeof(InvalidMeetingRoomOperationException) || e.GetType() == typeof(MeetingRoomException))
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
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
            AttendingCompany = meeting.AttendingCompany,
            Attendees = string.IsNullOrEmpty(meeting.Attendees?.Names) == false ? meeting.Attendees?.Names.Split(",") : null,
            MeetingRoom = new MeetingRoomViewModel()
            {
                CompanyId = meeting.MeetingRoom.CompanyId,
                Id = meeting.MeetingRoom.Id,
                Name = meeting.MeetingRoom.Name
            }
        };

        /// <summary>
        /// Get and map meeting rooms to viewmodels
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<MeetingRoomViewModel>> GetMeetingRooms()
        {
            // Get meeting rooms
            var meetingRooms = await _meetingRoomRepository.GetList(GetToken());

            return meetingRooms.Select(x => new MeetingRoomViewModel()
            {
                CompanyId = x.CompanyId,
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}
