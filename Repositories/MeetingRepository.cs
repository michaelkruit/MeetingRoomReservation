using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
using MeetingRooms.Exceptions;
using MeetingRooms.Interfaces;
using MeetingRooms.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Get all meetings of current that have a greater starting date greater or equal to today
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Meeting>> GetCompanyList(string token)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);
            // Get all meetings starting from today order by StartDatetime
            var meetings = await _dbContext.Meetings.Include(x => x.Attendees).Include(x => x.MeetingRoom)
                .Where(x => x.MeetingRoom.CompanyId == company.Id && x.StartDatetime > DateTime.Now.Date)
                .OrderBy(x => x.StartDatetime).ToArrayAsync();

            return meetings;
        }

        /// <summary>
        /// Get all meetings of selected meetingroom
        /// </summary>
        /// <param name="token"></param>
        /// <param name="meetingRoomId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Meeting>> GetMeetingRoomList(string token, int meetingRoomId)
        {
            // Get company and meeting room
            var company = _accountRepository.GetCompany(token);
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);

            // Check if meetingroom belongs to current company
            if (meetingRoom.CompanyId != company.Id)
            {
                throw new InvalidMeetingRoomOperationException("You are not allowd to get meetings for this meeting room");
            }

            // Get and return meetings
            var meetings = await _dbContext.Meetings.Include(x => x.Attendees).Include(x => x.MeetingRoom)
                .Where(x => x.MeetingRoomId == meetingRoomId && (x.StartDatetime > DateTime.Now || x.EndDatetime > DateTime.Now))
                .OrderBy(x => x.StartDatetime).ToArrayAsync();

            return meetings;
        }

        /// <summary>
        /// Get single meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Meeting> GetSingle(string token, int id)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);
            var meeting = await _dbContext.Meetings.Include(x => x.Attendees).Include(x => x.MeetingRoom).SingleOrDefaultAsync(x => x.Id == id) ??
                throw new InvalidMeetingRoomOperationException("Selected meeting room is not found");

            // Check if meeting belongs to current company
            if (meeting.MeetingRoom.CompanyId != company.Id)
            {
                throw new InvalidMeetingRoomOperationException("You are not allowd to get meetings for this meeting room");
            }

            return meeting;
        }

        /// <summary>
        /// Create new Meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public async Task<Meeting> Create(string token, MeetingCreateViewModel createModel)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);

            // Check if user is allowed to use meeting room
            if (await IsAllowedMeetingRoom(company, createModel.MeetingRoomId) == false)
            {
                throw new InvalidMeetingRoomOperationException("No allowed to set selected meeting room");
            }

            // Check if the end date is smaller
            if (InCorrectDates(createModel.StartDateTime, createModel.EndDateTime))
            {
                throw new MeetingRoomException("End date must be greater then the start date");
            }

            // Check if the selected meeting isn't reserverd 
            if (await IsOverLapping(createModel))
            {
                throw new MeetingRoomException("This meeting room has an overlapping reservation");
            }

            // Create new Meeting 
            var newMeeting = new Meeting()
            {
                MeetingRoomId = createModel.MeetingRoomId,
                AttendingCompany = createModel.AttendingCompany,
                StartDatetime = createModel.StartDateTime,
                EndDatetime = createModel.EndDateTime
            };

            // Check if any attendees are given
            if (createModel.Attendees?.Any() == true)
            {
                AddAttendees(newMeeting, createModel);
            }

            // Add and save 
            await _dbContext.AddAsync(newMeeting);
            await _dbContext.SaveChangesAsync();

            return newMeeting;
        }

        /// <summary>
        /// Delete selected Meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string token, int id)
        {
            // Get current comapny
            var company = _accountRepository.GetCompany(token);
            // Find Meeting
            var meeting = await _dbContext.Meetings.Include(x => x.MeetingRoom).SingleOrDefaultAsync(x => x.Id == id)
                ?? throw new MeetingRoomException("Meeting not found");
            // Check of meeting belongs to current company
            if (meeting.MeetingRoom.CompanyId != company.Id)
            {
                throw new InvalidMeetingRoomOperationException("You are not allowed to delete this meeting");
            }

            // Remove from db and save
            _dbContext.Remove(meeting);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Update existing meeting
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public async Task<Meeting> Update(string token, MeetingUpdateViewModel updateModel)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);

            // Check if user is allowed to use the selected meetingroom
            if (await IsAllowedMeetingRoom(company, updateModel.MeetingRoomId) == false)
            {
                throw new InvalidMeetingRoomOperationException("No allowed to set selected meeting room");
            }

            if (InCorrectDates(updateModel.StartDateTime, updateModel.EndDateTime))
            {
                throw new MeetingRoomException("End date must be greater then the start date");
            }

            // Check if meeting room is not reserved
            if (await IsOverLapping(updateModel))
            {
                throw new MeetingRoomException("This meeting room has an overlapping reservation");
            }

            // Get meeting 
            var meeting = await _dbContext.Meetings.Include(x => x.Attendees).SingleOrDefaultAsync(x => x.Id == updateModel.Id);

            AddRemoveOrUpdateAttendees(meeting, updateModel);

            // Update allowed properties
            meeting.MeetingRoomId = updateModel.MeetingRoomId;
            meeting.StartDatetime = updateModel.StartDateTime;
            meeting.EndDatetime = updateModel.EndDateTime;
            meeting.AttendingCompany = updateModel.AttendingCompany;

            // Update and save
            _dbContext.Update(meeting);
            await _dbContext.SaveChangesAsync();

            return meeting;
        }

        // Private helpers
        /// <summary>
        /// Check if user is allowed to use selected meetingroom
        /// </summary>
        /// <param name="company"></param>
        /// <param name="meetingRoomId"></param>
        /// <returns></returns>
        private async Task<bool> IsAllowedMeetingRoom(Company company, int meetingRoomId)
        {
            var meetingRoom = await _meetingRoomRepository.GetSingle(meetingRoomId);
            return meetingRoom.CompanyId == company.Id;
        }

        /// <summary>
        /// Check if new meeting doesn't overlap with current meetings
        /// </summary>
        /// <param name="createModel"></param>
        /// <returns></returns>
        private async Task<bool> IsOverLapping(MeetingCreateViewModel createModel)
            => await _dbContext.Meetings.AnyAsync(x => x.MeetingRoomId == createModel.MeetingRoomId
            && x.StartDatetime < createModel.EndDateTime
            && x.EndDatetime > createModel.StartDateTime);

        /// <summary>
        /// Check if existing meeting doesn't overlap with meetings
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        private async Task<bool> IsOverLapping(MeetingUpdateViewModel updateModel)
            => await _dbContext.Meetings.AnyAsync(x => x.Id != updateModel.Id
            && x.MeetingRoomId == updateModel.MeetingRoomId
            && x.StartDatetime < updateModel.EndDateTime
            && x.EndDatetime > updateModel.StartDateTime);

        /// <summary>
        ///  Check if end date is greater then the start date
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private bool InCorrectDates(DateTime start, DateTime end) => start > end;

        /// <summary>
        /// Add attendees to new meeting
        /// </summary>
        /// <param name="newMeeting"></param>
        /// <param name="createModel"></param>
        private void AddAttendees(Meeting newMeeting, MeetingCreateViewModel createModel)
        {
            // If all attendees are null, emtpy or just whitespace, don't do anything
            if (createModel.Attendees.All(x => string.IsNullOrWhiteSpace(x)))
            {
                return;
            }

            // Create new Attendees object
            newMeeting.Attendees = new Attendees()
            {
                // Add all attendees that have a value to names object 
                Names = string.Join(",", createModel.Attendees.Where(x => string.IsNullOrWhiteSpace(x) == false))
            };
        }

        /// <summary>
        /// Add remove or update attendees of an existing meeting
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="updateModel"></param>
        private void AddRemoveOrUpdateAttendees(Meeting meeting, MeetingUpdateViewModel updateModel)
        {
            // Current meeting doesn't have Attenddees and the update model also doesn't contain any
            if (meeting.Attendees == null && updateModel.Attendees?.Any() == false)
            {
                // don't do anything
                return;
            }
            // Current meeting doesn't have Attendees and the update model contains a valid list of attendees
            else if (meeting.Attendees == null && updateModel.Attendees?.All(x => string.IsNullOrWhiteSpace(x)) == false)
            {
                // Add new attendees
                meeting.Attendees = new Attendees()
                {
                    MeetingId = meeting.Id,
                    Names = string.Join(",", updateModel.Attendees.Where(x => string.IsNullOrWhiteSpace(x) == false))
                };
            }
            // Current meeting does containt attendees, but the update model doesn't contain any
            else if (meeting.Attendees != null && updateModel.Attendees == null)
            {
                // Remove attendees
                _dbContext.Remove(meeting.Attendees);
            }
            // Current meeting does have attendees and updatemodel has attendees
            else if (meeting.Attendees != null && updateModel.Attendees?.All(x => string.IsNullOrWhiteSpace(x)) == false)
            {
                // Update names
                meeting.Attendees.Names = string.Join(",", updateModel.Attendees.Where(x => string.IsNullOrWhiteSpace(x) == false));
            }
            // Current meetings does have attendees but update model contains a list of emtpy string
            return;
        }
    }
}
