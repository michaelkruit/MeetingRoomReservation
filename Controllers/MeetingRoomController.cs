﻿using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            var token = HttpContext.Session.GetString("Token");

            var meetingRooms = await _meetingRoomRepository.GetList(token);

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
        public async Task<IActionResult> Details(int id)
        {
            var meetingRoom = await _meetingRoomRepository.GetSingle(id);

            return View(meetingRoom);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
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

            var token = HttpContext.Session.GetString("Token");

            var meetingRoom = await _meetingRoomRepository.Create(token, model);

            return RedirectToAction(nameof(Details), new { meetingRoom.Id });
        }
    }
}
