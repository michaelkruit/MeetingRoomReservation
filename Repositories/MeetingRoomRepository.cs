using MeetingRooms.Data;
using MeetingRooms.Data.Entities;
using MeetingRooms.Exceptions;
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
    public class MeetingRoomRepository : IMeetingRoomRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountRepository _accountRepository;

        public MeetingRoomRepository(ApplicationDbContext dbContext, IAccountRepository accountRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Get list of meeting rooms
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MeetingRoom>> GetList(string token)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);
            // Get all meeting rooms of company
            var meetingRooms = await _dbContext.MeetingRooms.Where(x => x.CompanyId == company.Id).ToArrayAsync();
            return meetingRooms;
        }

        public async Task<MeetingRoom> GetSingle(int id)
            => await _dbContext.MeetingRooms.SingleOrDefaultAsync(x => x.Id == id) ?? throw new InvalidMeetingRoomOperationException("Meeting room not found");

        /// <summary>
        /// Create new meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="createModel"></param>
        /// <returns></returns>
        public async Task<MeetingRoom> Create(string token, MeetingRoomCreateViewModel createModel)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);

            // Check if meeting room not already exist in company
            if (await MeetingRoomExist(createModel.Name, company.Id, meetingRoomId: null))
            {
                throw new MeetingRoomException($"Meeting room with name '{createModel.Name}' already exist");
            }

            // Create new meeting room entity
            var meetingRoom = new MeetingRoom()
            {
                CompanyId = company.Id,
                Name = createModel.Name
            };

            // Add entity to db
            _dbContext.Add(meetingRoom);

            // Save
            await _dbContext.SaveChangesAsync();
            return meetingRoom;
        }

        /// <summary>
        /// Update meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public async Task<MeetingRoom> Update(string token, MeetingRoomUpdateViewModel updateModel)
        {
            // Get current company
            var company = _accountRepository.GetCompany(token);

            // Find meeting room
            var meetingRoom = await _dbContext.MeetingRooms.FindAsync(updateModel.Id) ??
                throw new MeetingRoomException($"Meeting room with name '{updateModel.Name}' not found");

            // Check if meeting room is linked to current company
            if (company.Id != meetingRoom.CompanyId)
            {
                throw new InvalidMeetingRoomOperationException("You are not allowed to update this meeting room");
            }

            // Check if meeting room already exist
            if (await MeetingRoomExist(updateModel.Name, company.Id, meetingRoom.Id))
            {
                throw new MeetingRoomException($"Meeting room with name '{updateModel.Name}' already exist in this company");
            }

            // Update the name
            meetingRoom.Name = updateModel.Name;

            // Update record in Db
            _dbContext.Update(meetingRoom);

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Return meeting room
            return meetingRoom;
        }

        /// <summary>
        /// Delete meeting room
        /// </summary>
        /// <param name="token"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string token, int id)
        {
            // Get company
            var company = _accountRepository.GetCompany(token);

            // Get meeting room
            var meetingRoom = await _dbContext.MeetingRooms.FindAsync(id) ??
                throw new InvalidMeetingRoomOperationException($"Meeting room not found");

            // Check if meeting room is linked to the users company
            if (company.Id != meetingRoom.CompanyId)
            {
                throw new InvalidMeetingRoomOperationException("You are not allowed to delete this meeting room");
            }

            // Remove and save
            _dbContext.Remove(meetingRoom);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Helper method to check if meeting room already exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="companyId"></param>
        /// <param name="meetingRoomId"></param>
        /// <returns></returns>
        private async Task<bool> MeetingRoomExist(string name, int companyId, int? meetingRoomId)
            => await _dbContext.MeetingRooms.AnyAsync(x => x.CompanyId == companyId
            && x.Name.ToLower() == name.ToLower()
            && (meetingRoomId.HasValue == false || x.Id != meetingRoomId.Value));
    }
}
