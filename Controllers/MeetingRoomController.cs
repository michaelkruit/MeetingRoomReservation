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
            var meetingRoomsViewModel = meetingRooms.Select(x => new MeetingRoomViewModel()
            {
                CompanyId = x.CompanyId,
                Id = x.Id,
                Name = x.Name
            });

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
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            // Create new meeting room
            var meetingRoom = await _meetingRoomRepository.Create(GetToken(), model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Get selected meeting room
            var meetingRoom = await _meetingRoomRepository.GetSingle(id);

            return View(meetingRoom);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingRoomUpdateViewModel model)
        {
            // Check if filled in model is valid
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update existing meeting room
            var meetingRoom = await _meetingRoomRepository.Update(GetToken(), model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Get selected meeting room
            var meetingRoom = await _meetingRoomRepository.GetSingle(id);

            // Create new meeting room viewmodel
            var meetingRoomViewModel = new MeetingRoomViewModel()
            {
                Id = id,
                Name = meetingRoom.Name
            };

            return View(meetingRoomViewModel);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            // Delete selected meeting room
            await _meetingRoomRepository.Delete(GetToken(), id);

            return RedirectToAction(nameof(Index));
        }

        // Helper to get token from session
        private string GetToken() => HttpContext.Session.GetString("Token");
    }
}
