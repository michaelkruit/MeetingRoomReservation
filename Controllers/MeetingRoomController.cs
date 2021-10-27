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
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var meetingRoom = await _meetingRoomRepository.Create(GetToken(), model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var meetingRoom = await _meetingRoomRepository.GetSingle(id);

            return View(meetingRoom);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MeetingRoomUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var meetingRoom = await _meetingRoomRepository.Update(GetToken(), model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var meetingRoom = await _meetingRoomRepository.GetSingle(id);

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
            await _meetingRoomRepository.Delete(GetToken(), id);

            return RedirectToAction(nameof(Index));
        }

        private string GetToken() => HttpContext.Session.GetString("Token");
    }
}
