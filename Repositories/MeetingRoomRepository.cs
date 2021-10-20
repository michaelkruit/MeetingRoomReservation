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
    public class MeetingRoomRepository : IMeetingRoomRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public MeetingRoomRepository(ApplicationDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task<MeetingRoom> Create(string token, MeetingRoomCreateViewModel createModel)
        {
            var company = GetCompany(token);
            var meetingRoom = new MeetingRoom()
            {
                CompanyId = company.Id,
                Name = createModel.Name
            };
            _dbContext.Add(meetingRoom);
            if(await _dbContext.SaveChangesAsync() > 0)
            {
                return meetingRoom;
            }
            throw new Exception("Meeting room didn't save");
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MeetingRoom>> GetList(string token)
        {
            var company = GetCompany(token);
            var meetingRooms = await _dbContext.MeetingRooms.Where(x => x.CompanyId == company.Id).ToArrayAsync();
            return meetingRooms;
        }

        public async Task<MeetingRoom> GetSingle(int id)
            => await _dbContext.MeetingRooms.SingleOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Meeting room not found");

        public Task<MeetingRoom> Update(MeetingRoomUpdateViewModel updateModel)
        {
            throw new NotImplementedException();
        }

        private Company GetCompany(string token)
            => _memoryCache.Get<Company>(token) ?? throw new Exception("No company found in cache");
    }
}
