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
    public class MeetingRoomController : Controller
    {
        private readonly IMeetingRoomRepository _meetingRoomRepository;

        public MeetingRoomController(IMeetingRoomRepository meetingRoomRepository)
        {
            _meetingRoomRepository = meetingRoomRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get meeting rooms
            var meetingRooms = await _meetingRoomRepository.GetList(GetToken());

            // Map to viewmodel
            var meetingRoomsViewModel = meetingRooms.Select(x => MapToViewModel(x));

            return View(meetingRoomsViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(MeetingRoomCreateViewModel model)
        {
            // Check if filed in model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create new meeting room
                var meetingRoom = await _meetingRoomRepository.Create(GetToken(), model);

                return RedirectToAction(nameof(Index));
            }
            catch (MeetingRoomException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
            catch
            {
               return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                // Get selected meeting room
                var meetingRoom = await _meetingRoomRepository.GetSingle(id);

                var viewModel = MapToViewModel(meetingRoom);

                return View(viewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingRoomUpdateViewModel model)
        {
            // Check if filled in model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Update existing meeting room
                var meetingRoom = await _meetingRoomRepository.Update(GetToken(), model);

                return RedirectToAction(nameof(Index));
            }
            catch (MeetingRoomException e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
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
                // Get selected meeting room
                var meetingRoom = await _meetingRoomRepository.GetSingle(id);

                // Create new meeting room viewmodel
                var meetingRoomViewModel = MapToViewModel(meetingRoom);

                return View(meetingRoomViewModel);
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            try
            {
                // Delete selected meeting room
                await _meetingRoomRepository.Delete(GetToken(), id);

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidMeetingRoomOperationException e)
            {
                return RedirectToAction("Error", "Home", new { errorMessage = e.Message });
            }
        }

        // Helper to get token from session
        private string GetToken() => HttpContext.Session.GetString("Token");

        private MeetingRoomViewModel MapToViewModel(MeetingRoom meetingRoom)
            => new MeetingRoomViewModel()
            {
                CompanyId = meetingRoom.CompanyId,
                Id = meetingRoom.Id,
                Name = meetingRoom.Name
            };
    }
}
