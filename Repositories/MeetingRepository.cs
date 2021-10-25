using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingRooms.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountRepository _accountRepository;
        private readonly IMeetingRoomRepository _meetingRoomRepository;

        public MeetingRepository(ApplicationDbContext dbContext, IAccountRepository accountRepository, IMeetingRoomRepository meetingRoomRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _meetingRoomRepository = meetingRoomRepository;
        }

        public async Task<Meeting> Create(string token, MeetingCreateViewModel createModel)
        {
            var company = _accountRepository.GetCompany(token);

            if (await IsAllowedMeetingRoom(company, createModel.MeetingRoomId) == false)
            {
                throw new InvalidOperationException("No allowed to set selected meeting room");
            }

            if (await IsOverLapping(createModel))
            {
                throw new InvalidOperationException("This meeting room has an overlapping reservation");
            }

            var newMeeting = new Meeting()
            {
                MeetingRoomId = createModel.MeetingRoomId,
                AttendingCompany = createModel.AttendingCompany,
                StartDatetime = createModel.StartDateTime,
                EndDatetime = createModel.EndDateTime
            };

            await _dbContext.AddAsync(newMeeting);
            await _dbContext.SaveChangesAsync();

            return newMeeting;
        }

        public async Task<bool> Delete(string token, int id)
        {
            var company = _accountRepository.GetCompany(token);
            var meeting = await _dbContext.Meetings.FindAsync(id)
                ?? throw new NullReferenceException("Meeting not found");
            var meetingRoom = await _meetingRoomRepository.GetSingle(meeting.MeetingRoomId);
            if(meetingRoom.CompanyId != company.Id)
            {
                throw new AccessViolationException("You are not allowed to delete this meeting");
            }

            _dbContext.Remove(meeting);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Meeting>> GetCompanyList(string token)
        {
            var company = _accountRepository.GetCompany(token);
            var meetings = await _dbContext.Meetings.Where(x => x.MeetingRoom.CompanyId == company.Id && x.StartDatetime > DateTime.Now.Date).ToArrayAsync();

            return meetings;
        }

        public async Task<IEnumerable<Meeting>> GetMeetingRoomList(string token, int meetingRoomId)
        {
            var company = _accountRepository.GetCompany(token);
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);
            
            if(meetingRoom.CompanyId != company.Id)
            {
                throw new AccessViolationException("You are not allowd to get meetings for this meeting room");
            }
            
            var meetings = await _dbContext.Meetings.Where(x => x.MeetingRoomId == meetingRoomId && x.StartDatetime > DateTime.Now.Date).ToArrayAsync();

            return meetings;
        }

        public async Task<Meeting> GetSingle(string token, int id)
        {
            var company = _accountRepository.GetCompany(token);
            var meeting = await _dbContext.Meetings.FindAsync(id);
            var meetingRoom = await _meetingRoomRepository.GetSingle(meeting.MeetingRoomId);

            if (meetingRoom.CompanyId != company.Id)
            {
                throw new AccessViolationException("You are not allowd to get meetings for this meeting room");
            }

            return meeting;
        }

        public Task<Meeting> Update(string token, MeetingUpdateViewModel updateModel)
        {
            throw new NotImplementedException();
        }

        // Private helpers
        private async Task<bool> IsAllowedMeetingRoom(Company company, int meetingRoomId)
        {
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);
            return meetingRoom.CompanyId == company.Id;
        }

        private Task<bool> IsOverLapping(MeetingCreateViewModel createModel)
        {
            var isOverLapping = _dbContext.Meetings.AnyAsync(x => x.MeetingRoomId == createModel.MeetingRoomId
            && x.StartDatetime < createModel.EndDateTime
            && x.EndDatetime > createModel.StartDateTime);

            return isOverLapping;
        }
    }
}
